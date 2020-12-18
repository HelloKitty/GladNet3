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
	/// Base TCP <see cref="SocketConnection"/>-based <see cref="ManagedSession{TPayloadWriteType,TPayloadReadType}"/>
	/// implementation.
	/// </summary>
	/// <typeparam name="TPayloadWriteType"></typeparam>
	/// <typeparam name="TPayloadReadType"></typeparam>
	public abstract class BaseTcpManagedSession<TPayloadWriteType, TPayloadReadType> 
		: ManagedSession<TPayloadWriteType, TPayloadReadType>, INetworkMessageReceivable<TPayloadReadType>
		where TPayloadWriteType : class 
		where TPayloadReadType : class
	{
		protected NetworkConnectionOptions NetworkOptions { get; }

		/// <summary>
		/// The socket connection.
		/// </summary>
		protected SocketConnection Connection { get; }

		/// <summary>
		/// The factory for building packet headers.
		/// </summary>
		private IPacketHeaderFactory PacketHeaderFactory { get; }

		/// <summary>
		/// The incoming message deserializer.
		/// </summary>
		private IMessageDeserializer<TPayloadReadType> MessageDeserializer { get; }

		/// <summary>
		/// The incoming message deserializer.
		/// </summary>
		private IMessageSerializer<TPayloadWriteType> MessageSerializer { get; }

		/// <summary>
		/// The outgoing message header serializer.
		/// </summary>
		private IMessageSerializer<PacketHeaderSerializationContext<TPayloadWriteType>> HeaderSerializer { get; }

		/// <summary>
		/// The outgoing message queue.
		/// </summary>
		private AsyncProducerConsumerQueue<TPayloadWriteType> OutgoingMessageQueue { get; } = new AsyncProducerConsumerQueue<TPayloadWriteType>();

		protected BaseTcpManagedSession(NetworkConnectionOptions networkOptions, SocketConnection connection, SessionDetails details, 
			IPacketHeaderFactory packetHeaderFactory, 
			IMessageDeserializer<TPayloadReadType> messageDeserializer, 
			IMessageSerializer<TPayloadWriteType> messageSerializer, 
			IMessageSerializer<PacketHeaderSerializationContext<TPayloadWriteType>> headerSerializer) 
			: base(new SocketConnectionConnectionServiceAdapter(connection), details)
		{
			NetworkOptions = networkOptions ?? throw new ArgumentNullException(nameof(networkOptions));
			Connection = connection ?? throw new ArgumentNullException(nameof(connection));
			PacketHeaderFactory = packetHeaderFactory ?? throw new ArgumentNullException(nameof(packetHeaderFactory));
			MessageDeserializer = messageDeserializer ?? throw new ArgumentNullException(nameof(messageDeserializer));
			MessageSerializer = messageSerializer ?? throw new ArgumentNullException(nameof(messageSerializer));
			HeaderSerializer = headerSerializer ?? throw new ArgumentNullException(nameof(headerSerializer));
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
						continue;

					if (!PacketHeaderFactory.IsHeaderReadable(in buffer))
						continue;

					IPacketHeader header = ReadIncomingPacketHeader(Connection.Input, in result);

					do
					{
						//Now with the header we know how much data we must now read for the payload.
						result = await Connection.Input.ReadAsync(token);

						//This call will also use the cancel token so we don't need to check it in the nested-loop.
						if (!IsReadResultValid(in result))
							return;

					} while (result.Buffer.Length < header.PayloadSize); //We need at least PAYLOAD SIZE bytes otherwise we cannot read the payload.

					//TODO: Valid incoming packet lengths to avoid a stack overflow.
					//This point we have a VALID read result that is NOT less than header.PayloadSize
					//therefore it should be safe now to read the incoming packet.
					TPayloadReadType message = ReadIncomingPacketPayload(Connection.Input, in result, header.PayloadSize);
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

		private unsafe TPayloadReadType ReadIncomingPacketPayload(PipeReader reader, in ReadResult result, int payloadSize)
		{
			//I opted to do this instead of stack alloc because of HUGE dangers in stack alloc and this if pretty efficient
			//buffer usage anyway.
			byte[] rentedBuffer = ArrayPool<byte>.Shared.Rent(payloadSize);
			Span<byte> buffer = new Span<byte>(rentedBuffer, 0, payloadSize);

			try
			{
				result.Buffer.Slice(0, payloadSize).CopyTo(buffer);

				int offset = 0;
				TPayloadReadType message = MessageDeserializer.Deserialize(buffer, ref offset);

				//Steps the reader N bytes forward (if we don't do this WE LEAK AND WORSE!!)
				reader.AdvanceTo(result.Buffer.GetPosition(offset));
				return message;
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(rentedBuffer);
			}

		}

		private IPacketHeader ReadIncomingPacketHeader(PipeReader reader, in ReadResult result)
		{
			//The implementation MUST be that this can be trusted to be the EXACT size of binary data that will be read.
			int exactHeaderByteCount = PacketHeaderFactory.ComputeHeaderSize(result.Buffer);

			IPacketHeader header;
			using(var context = new PacketHeaderCreationContext(result.Buffer, exactHeaderByteCount))
				header = PacketHeaderFactory.Create(context);

			//Steps the reader N bytes forward (if we don't do this WE LEAK AND WORSE!!)
			reader.AdvanceTo(result.Buffer.GetPosition(exactHeaderByteCount));
			return header;
		}

		private bool IsReadResultValid(in ReadResult result)
		{
			//TODO: Does this mean it's DONE??
			if (result.IsCanceled || result.IsCompleted)
			{
				Connection.Input.AdvanceTo(result.Buffer.Start, result.Buffer.End);
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
				await Connection.Input.CompleteAsync(e);
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
			HeaderSerializer.Serialize(new PacketHeaderSerializationContext<TPayloadWriteType>(payload, payloadSize), buffer, ref headerOffset);
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
			MessageSerializer.Serialize(payload, buffer, ref offset);
			return offset;
		}

		/// <inheritdoc />
		public abstract Task OnNetworkMessageReceived(NetworkIncomingMessage<TPayloadReadType> message);
	}
}
