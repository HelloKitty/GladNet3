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
		: ManagedSession<TPayloadReadType, TPayloadWriteType>
		where TPayloadWriteType : class 
		where TPayloadReadType : class
	{
		/// <summary>
		/// The socket connection.
		/// </summary>
		protected SocketConnection Connection { get; }

		/// <summary>
		/// The outgoing message queue.
		/// </summary>
		protected AsyncProducerConsumerQueue<TPayloadWriteType> OutgoingMessageQueue { get; } = new AsyncProducerConsumerQueue<TPayloadWriteType>();

		protected BaseTcpManagedSession(NetworkConnectionOptions networkOptions, SocketConnection connection, SessionDetails details,
			SessionMessageBuildingServiceContext<TPayloadWriteType, TPayloadReadType> messageServices) 
			: base(new SocketConnectionConnectionServiceAdapter(connection), details, networkOptions,
				new SocketConnectionNetworkMessageProducer<TPayloadWriteType, TPayloadReadType>(networkOptions, connection, messageServices),
				messageServices)
		{
			Connection = connection ?? throw new ArgumentNullException(nameof(connection));
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

		/// <inheritdoc />
		public override async Task StartWritingAsync(CancellationToken token = default)
		{
			try
			{
				while (!token.IsCancellationRequested)
				{
					//Dequeue from the outgoing message queue and send through the send service.
					TPayloadWriteType payload = await OutgoingMessageQueue.DequeueAsync(token);
					SendResult result = await NetworkMessageInterface.SendMessageAsync(payload, token);

					//TODO: Add logging!
					if (result != SendResult.Sent && result != SendResult.Enqueued)
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
	}
}
