using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glader.Essentials;
using Nito.AsyncEx;
using Pipelines.Sockets.Unofficial;

namespace GladNet
{
	/// <summary>
	/// Base TCP <see cref="SocketConnection"/>-based <see cref="ManagedSession"/>
	/// implementation.
	/// </summary>
	/// <typeparam name="TPayloadWriteType"></typeparam>
	/// <typeparam name="TPayloadReadType"></typeparam>
	public abstract class BaseTcpManagedSession<TPayloadWriteType, TPayloadReadType> 
		: ManagedSession, INetworkMessageReceivable<TPayloadReadType>
		where TPayloadWriteType : class 
		where TPayloadReadType : class
	{
		/// <summary>
		/// The socket connection.
		/// </summary>
		protected SocketConnection Connection { get; }

		/// <summary>
		/// Message serialization/building services.
		/// </summary>
		protected SessionMessageServiceContext<TPayloadWriteType, TPayloadReadType> MessageServices { get; }

		/// <summary>
		/// The outgoing message queue.
		/// </summary>
		private AsyncProducerConsumerQueue<TPayloadWriteType> OutgoingMessageQueue { get; } = new AsyncProducerConsumerQueue<TPayloadWriteType>();

		protected BaseTcpManagedSession(NetworkConnectionOptions networkOptions, SocketConnection connection, SessionDetails details,
			SessionMessageServiceContext<TPayloadWriteType, TPayloadReadType> messageServices) 
			: base(new SocketConnectionConnectionServiceAdapter(connection), details, networkOptions)
		{
			Connection = connection ?? throw new ArgumentNullException(nameof(connection));
			MessageServices = messageServices ?? throw new ArgumentNullException(nameof(messageServices));
		}

		/// <inheritdoc />
		public override async Task StartListeningAsync(CancellationToken token = default)
		{
			try
			{
				while (!token.IsCancellationRequested)
				{
					ReadResult result = await Connection.Input.ReadAsync(token);

					if (!IsReadResultValid(in result))
						return;

					ReadOnlySequence<byte> buffer = result.Buffer;

					//So we have a valid result, let's check if we have enough data.
					//If we don't have enough to read a packet header then we need to wait until we have enough bytes.
					if (buffer.Length < NetworkOptions.MinimumPacketHeaderSize)
					{
						//If buffer isn't large enough we need to tell Pipeline we didn't consume anything
						//but we DID inspect/examine all the way to the end of the buffer.
						Connection.Input.AdvanceTo(result.Buffer.Start, result.Buffer.End);
						continue;
					}

					if (!MessageServices.PacketHeaderFactory.IsHeaderReadable(in buffer))
					{
						//If buffer isn't large enough we need to tell Pipeline we didn't consume anything
						//but we DID inspect/examine all the way to the end of the buffer.
						Connection.Input.AdvanceTo(result.Buffer.Start, result.Buffer.End);
						continue;
					}

					IPacketHeader header = ReadIncomingPacketHeader(Connection.Input, in result);

					//TODO: Add logging
					if (!IsHeaderValid(header))
						return;

					while (!token.IsCancellationRequested)
					{
						//Now with the header we know how much data we must now read for the payload.
						result = await Connection.Input.ReadAsync(token);

						//This call will also use the cancel token so we don't need to check it in the nested-loop.
						if(!IsReadResultValid(in result))
							return;

						//This is dumb and hacky, but we need to know if we shall need to read again
						//This means we're still at the START of the buffer (haven't read anything)
						//but have technically aware/inspected to the END position.
						if(result.Buffer.Length < header.PayloadSize)
							Connection.Input.AdvanceTo(result.Buffer.Start, result.Buffer.End);
						else
							break;
					}

					TPayloadReadType message;
					try
					{
						//TODO: Valid incoming packet lengths to avoid a stack overflow.
						//This point we have a VALID read result that is NOT less than header.PayloadSize
						//therefore it should be safe now to read the incoming packet.
						message = ReadIncomingPacketPayload(in result, header.PayloadSize);
					}
					catch (Exception)
					{
						//TODO: We should log WHY but basically we should no longer continue the network listener.
						return;
					}
					finally
					{
						//So serialization should output an offset read. However, we should not use
						//that as the basis for the bytes read. Serialization can be WRONG and conflict with
						//the packet header's defined size. Therefore, we should trust packet header over serialization
						//logic ALWAYS and this advance should be in a finally block for sure.
						Connection.Input.AdvanceTo(result.Buffer.GetPosition(header.PayloadSize));
					}
					
					//A throw will stop the session.
					await OnNetworkMessageReceived(new NetworkIncomingMessage<TPayloadReadType>(header, message));
				}
			}
			catch (ConnectionResetException e)
			{
				//ConnectionResetException is thrown by Pipeline Socket API when it disconnects
				//and I don't yet know how to suppress it fully.
				//Consider a cancel a graceful complete
				await Connection.Input.CompleteAsync(e);
				return;
			}
			catch (TaskCanceledException e)
			{
				//Consider a cancel a graceful complete
				await Connection.Input.CompleteAsync(e);
				return;
			}
			catch (Exception e)
			{
				await Connection.Input.CompleteAsync(e);
				throw;
			}
			finally
			{
				await Connection.Input.CompleteAsync();
			}
		}

		private TPayloadReadType ReadIncomingPacketPayload(in ReadResult result, int payloadSize)
		{
			//I opted to do this instead of stack alloc because of HUGE dangers in stack alloc and this is pretty efficient
			//buffer usage anyway.
			byte[] rentedBuffer = ArrayPool<byte>.Shared.Rent(payloadSize);
			Span<byte> buffer = new Span<byte>(rentedBuffer, 0, payloadSize);

			try
			{
				//This copy is BAD but it really avoids a lot of API headaches
				result.Buffer.Slice(0, payloadSize).CopyTo(buffer);

				int offset = 0;
				return MessageServices.MessageDeserializer.Deserialize(buffer, ref offset);
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(rentedBuffer);
			}
		}

		private IPacketHeader ReadIncomingPacketHeader(PipeReader reader, in ReadResult result)
		{
			int exactHeaderByteCount = 0;
			try
			{
				//The implementation MUST be that this can be trusted to be the EXACT size of binary data that will be read.
				exactHeaderByteCount = MessageServices.PacketHeaderFactory.ComputeHeaderSize(result.Buffer);

				IPacketHeader header;
				using(var context = new PacketHeaderCreationContext(result.Buffer, exactHeaderByteCount))
					header = MessageServices.PacketHeaderFactory.Create(context);

				return header;
			}
			finally
			{
				//Advance to only the exact header bytes read, consumed and examined. Do not use buffer lengths ever!
				reader.AdvanceTo(result.Buffer.GetPosition(exactHeaderByteCount));
			}
		}

		private bool IsReadResultValid(in ReadResult result)
		{
			//TODO: Does this mean it's DONE??
			if (result.IsCanceled || result.IsCompleted)
			{
				//This means we CONSUMED to end of buffer and INSPECTED to end of buffer
				//We're DONE with all read buffer data.
				Connection.Input.AdvanceTo(result.Buffer.End);
				return false;
			}

			return true;
		}

		private bool IsFlushResultValid(in FlushResult result)
		{
			//TODO: Does this mean it's DONE??
			if(result.IsCanceled || result.IsCompleted)
				return false;

			return true;
		}

		/// <inheritdoc />
		public override async Task StartWritingAsync(CancellationToken token = default)
		{
			try
			{
				while (!token.IsCancellationRequested)
				{
					TPayloadWriteType payload = await OutgoingMessageQueue.DequeueAsync(token);

					WriteOutgoingMessage(payload);

					//To understand the purpose of Flush when pipelines is using sockets see Marc's comments here: https://stackoverflow.com/questions/56481746/does-pipelines-sockets-unofficial-socketconnection-ever-flush-without-a-request
					//Basically, "it makes sure that a consumer is awakened (if it isn't already)" and "if there is back-pressure, it delays the producer until the consumer has cleared some of the back-pressure"
					FlushResult result = await Connection.Output.FlushAsync(token);

					if (!IsFlushResultValid(in result))
						return;
				}
			}
			catch (ConnectionResetException e)
			{
				//ConnectionResetException is thrown by Pipeline Socket API when it disconnects
				//and I don't yet know how to suppress it fully.
				//Consider a cancel a graceful complete
				await Connection.Output.CompleteAsync(e);
				return;
			}
			catch(TaskCanceledException e)
			{
				//Consider a cancel a graceful complete
				await Connection.Output.CompleteAsync(e);
				return;
			}
			catch(Exception e)
			{
				await Connection.Output.CompleteAsync(e);
				throw;
			}
			finally
			{
				await Connection.Output.CompleteAsync();
			}
		}

		private void WriteOutgoingMessage(TPayloadWriteType payload)
		{
			if (payload == null) throw new ArgumentNullException(nameof(payload));

			//TODO: We should find a way to predict the size of a payload type.
			Span<byte> buffer = Connection.Output.GetSpan(NetworkOptions.MaximumPacketSize);

			//It seems backwards, but we don't know what header to build until the payload is serialized.
			int payloadSize = SerializeOutgoingPacketPayload(buffer.Slice(NetworkOptions.MinimumPacketHeaderSize), payload);
			int headerSize = SerializeOutgoingHeader(payload, payloadSize, buffer.Slice(0, NetworkOptions.MaximumPacketHeaderSize));

			//TODO: We must eventually support VARIABLE LENGTH packet headers. This is complicated, WoW does this for large packets sent by the server.
			if (headerSize != NetworkOptions.MinimumPacketHeaderSize)
				throw new NotSupportedException($"TODO: Variable length packet header sizes are not yet supported.");

			Connection.Output.Advance(payloadSize + headerSize);
		}

		private int SerializeOutgoingHeader(TPayloadWriteType payload, int payloadSize, in Span<byte> buffer)
		{
			int headerOffset = 0;
			MessageServices.HeaderSerializer.Serialize(new PacketHeaderSerializationContext<TPayloadWriteType>(payload, payloadSize), buffer, ref headerOffset);
			return headerOffset;
		}

		/// <summary>
		/// Writes the outgoing packet payload.
		/// Returns the number of bytes the payload was sent as.
		/// </summary>
		/// <param name="buffer">The buffer to write the packet payload to.</param>
		/// <param name="payload">The payload instance.</param>
		/// <returns>The number of bytes the payload was sent as.</returns>
		private int SerializeOutgoingPacketPayload(in Span<byte> buffer, TPayloadWriteType payload)
		{
			//Serializes the payload data to the span buffer and moves the pipe forward by the ref output offset
			//meaning we indicate to the pipeline that we've written bytes
			int offset = 0;
			MessageServices.MessageSerializer.Serialize(payload, buffer, ref offset);
			return offset;
		}

		//Warning to implementer, if you THROW from this you WILL stop the network connection completely.
		//GladNet does not sustain exceptions in unexpected cases, choosing to shutdown the session instead.
		/// <inheritdoc />
		public abstract Task OnNetworkMessageReceived(NetworkIncomingMessage<TPayloadReadType> message);
	}
}
