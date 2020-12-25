using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Pipelines.Sockets.Unofficial;

namespace GladNet
{
	/// <summary>
	/// TCP <see cref="SocketConnection"/> Pipelines-based implementation of <see cref="INetworkMessageInterface{TPayloadReadType,TPayloadWriteType}"/>
	/// </summary>
	/// <typeparam name="TPayloadWriteType"></typeparam>
	/// <typeparam name="TPayloadReadType"></typeparam>
	public sealed class SocketConnectionNetworkMessageInterface<TPayloadReadType, TPayloadWriteType> : INetworkMessageInterface<TPayloadReadType, TPayloadWriteType>
		where TPayloadWriteType : class 
		where TPayloadReadType : class
	{
		/// <summary>
		/// The pipelines socket connection.
		/// </summary>
		private SocketConnection Connection { get; }

		/// <summary>
		/// The messages service container.
		/// </summary>
		private SessionMessageBuildingServiceContext<TPayloadReadType, TPayloadWriteType> MessageServices { get; }

		/// <summary>
		/// The details of the session.
		/// </summary>
		private NetworkConnectionOptions NetworkOptions { get; }

		private AsyncLock PayloadWriteLock { get; } = new AsyncLock();

		public SocketConnectionNetworkMessageInterface(NetworkConnectionOptions networkOptions, 
			SocketConnection connection, 
			SessionMessageBuildingServiceContext<TPayloadReadType, TPayloadWriteType> messageServices)
		{
			Connection = connection ?? throw new ArgumentNullException(nameof(connection));
			MessageServices = messageServices ?? throw new ArgumentNullException(nameof(messageServices));
			NetworkOptions = networkOptions ?? throw new ArgumentNullException(nameof(networkOptions));
		}

		public async Task<NetworkIncomingMessage<TPayloadReadType>> ReadMessageAsync(CancellationToken token = default(CancellationToken))
		{
			while (!token.IsCancellationRequested)
			{
				ReadResult result;

				//The reason we wrap this in a try/catch is because the Pipelines may abort ungracefully
				//inbetween our state checks. Therefore only calling ReadAsyc can truly indicate the state of a connection
				//and if it has been aborted then we should pretend as if we read nothing.
				try
				{
					result = await Connection.Input.ReadAsync(token);
				}
				catch(ConnectionAbortedException abortException)
				{
					return null;
				}

				if (!IsReadResultValid(in result))
					return null;

				ReadOnlySequence<byte> buffer = result.Buffer;

				//So we have a valid result, let's check if we have enough data.
				//If we don't have enough to read a packet header then we need to wait until we have enough bytes.
				if(buffer.Length < NetworkOptions.MinimumPacketHeaderSize)
				{
					//If buffer isn't large enough we need to tell Pipeline we didn't consume anything
					//but we DID inspect/examine all the way to the end of the buffer.
					Connection.Input.AdvanceTo(result.Buffer.Start, result.Buffer.End);
					continue;
				}

				if(!MessageServices.PacketHeaderFactory.IsHeaderReadable(in buffer))
				{
					//If buffer isn't large enough we need to tell Pipeline we didn't consume anything
					//but we DID inspect/examine all the way to the end of the buffer.
					Connection.Input.AdvanceTo(result.Buffer.Start, result.Buffer.End);
					continue;
				}

				IPacketHeader header = ReadIncomingPacketHeader(Connection.Input, in result);

				//TODO: Add header validation.

				while(!token.IsCancellationRequested)
				{
					//The reason we wrap this in a try/catch is because the Pipelines may abort ungracefully
					//inbetween our state checks. Therefore only calling ReadAsyc can truly indicate the state of a connection
					//and if it has been aborted then we should pretend as if we read nothing.
					try
					{
						//Now with the header we know how much data we must now read for the payload.
						result = await Connection.Input.ReadAsync(token);
					}
					catch(ConnectionAbortedException abortException)
					{
						return null;
					}

					//This call will also use the cancel token so we don't need to check it in the nested-loop.
					if (!IsReadResultValid(in result))
						return null;

					//This is dumb and hacky, but we need to know if we shall need to read again
					//This means we're still at the START of the buffer (haven't read anything)
					//but have technically aware/inspected to the END position.
					if(result.Buffer.Length < header.PayloadSize)
						Connection.Input.AdvanceTo(result.Buffer.Start, result.Buffer.End);
					else
						break;
				}

				try
				{
					//TODO: Valid incoming packet lengths to avoid a stack overflow.
					//This point we have a VALID read result that is NOT less than header.PayloadSize
					//therefore it should be safe now to read the incoming packet.
					TPayloadReadType payload = ReadIncomingPacketPayload(in result, header.PayloadSize);

					return new NetworkIncomingMessage<TPayloadReadType>(header, payload);
				}
				catch(Exception)
				{
					//TODO: We should log WHY but basically we should no longer continue the network listener.
					return null;
				}
				finally
				{
					//So serialization should output an offset read. However, we should not use
					//that as the basis for the bytes read. Serialization can be WRONG and conflict with
					//the packet header's defined size. Therefore, we should trust packet header over serialization
					//logic ALWAYS and this advance should be in a finally block for sure.
					Connection.Input.AdvanceTo(result.Buffer.GetPosition(header.PayloadSize));
				}
			}

			return null;
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
			if(result.IsCanceled || result.IsCompleted)
			{
				//This means we CONSUMED to end of buffer and INSPECTED to end of buffer
				//We're DONE with all read buffer data.
				Connection.Input.AdvanceTo(result.Buffer.End);
				return false;
			}

			return true;
		}

		/// <inheritdoc />
		public async Task<SendResult> SendMessageAsync(TPayloadWriteType message, CancellationToken token = default)
		{
			if(!Connection.Socket.Connected)
				return SendResult.Disconnected;

			//THIS IS CRITICAL, IT'S NOT SAFE TO SEND MULTIPLE THREADS AT ONCE!!
			using (await PayloadWriteLock.LockAsync(token))
			{
				try
				{
					WriteOutgoingMessage(message);

					//To understand the purpose of Flush when pipelines is using sockets see Marc's comments here: https://stackoverflow.com/questions/56481746/does-pipelines-sockets-unofficial-socketconnection-ever-flush-without-a-request
					//Basically, "it makes sure that a consumer is awakened (if it isn't already)" and "if there is back-pressure, it delays the producer until the consumer has cleared some of the back-pressure"
					FlushResult result = await Connection.Output.FlushAsync(token);

					if(!IsFlushResultValid(in result))
						return SendResult.Error;
				}
				catch (Exception e)
				{
					//TODO: Logging!
					return SendResult.Error;
				}

				return SendResult.Sent;
			}
		}

		private void WriteOutgoingMessage(TPayloadWriteType payload)
		{
			if(payload == null) throw new ArgumentNullException(nameof(payload));

			//TODO: We should find a way to predict the size of a payload type.
			Span<byte> buffer = Connection.Output.GetSpan(NetworkOptions.MaximumPacketSize);

			//It seems backwards, but we don't know what header to build until the payload is serialized.
			int payloadSize = SerializeOutgoingPacketPayload(buffer.Slice(NetworkOptions.MinimumPacketHeaderSize), payload);
			int headerSize = SerializeOutgoingHeader(payload, payloadSize, buffer.Slice(0, NetworkOptions.MaximumPacketHeaderSize));

			//TODO: We must eventually support VARIABLE LENGTH packet headers. This is complicated, WoW does this for large packets sent by the server.
			if(headerSize != NetworkOptions.MinimumPacketHeaderSize)
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

		private bool IsFlushResultValid(in FlushResult result)
		{
			//TODO: Does this mean it's DONE??
			if(result.IsCanceled || result.IsCompleted)
				return false;

			return true;
		}
	}
}
