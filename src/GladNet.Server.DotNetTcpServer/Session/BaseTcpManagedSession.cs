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
	public abstract class BaseTcpManagedSession<TPayloadReadType, TPayloadWriteType> 
		: ManagedSession<TPayloadReadType, TPayloadWriteType>, INetworkMessageReceivable<TPayloadReadType>
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
		protected AsyncProducerConsumerQueue<TPayloadWriteType> OutgoingMessageQueue { get; } = new AsyncProducerConsumerQueue<TPayloadWriteType>();

		protected BaseTcpManagedSession(NetworkConnectionOptions networkOptions, SocketConnection connection, SessionDetails details,
			SessionMessageServiceContext<TPayloadWriteType, TPayloadReadType> messageServices) 
			: base(new SocketConnectionConnectionServiceAdapter(connection), details, networkOptions,
				new SocketConnectionNetworkMessageProducer<TPayloadWriteType, TPayloadReadType>(networkOptions, connection, messageServices))
		{
			Connection = connection ?? throw new ArgumentNullException(nameof(connection));
			MessageServices = messageServices ?? throw new ArgumentNullException(nameof(messageServices));
		}

		/// <inheritdoc />
		public override async Task StartListeningAsync(CancellationToken token = default)
		{
			//We override this to add some SocketConnection handling on finishing.
			try
			{
				await base.StartListeningAsync(token);
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
	}
}
