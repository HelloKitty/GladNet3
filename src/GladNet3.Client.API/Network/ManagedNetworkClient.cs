using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using Common.Logging;
using Common.Logging.Simple;

namespace GladNet
{
	/// <summary>
	/// High level client for consumption that manages the lower level networking.
	/// Provides a high level, typesafe API with simple interactions.
	/// </summary>
	public sealed class ManagedNetworkClient<TClientType, TPayloadWriteType, TPayloadReadType> : ManagedNetworkClientBase<TClientType, TPayloadWriteType, TPayloadReadType>
		where TPayloadWriteType : class
		where TPayloadReadType : class
		where TClientType : class, IDisconnectable, IConnectable, IPacketPayloadWritable<TPayloadWriteType>, IPacketPayloadReadable<TPayloadReadType>
	{
		/// <summary>
		/// The outgoing message queue.
		/// </summary>
		private AsyncProducerConsumerQueue<TPayloadWriteType> OutgoingMessageQueue { get; }

		/// <summary>
		/// The incomding message queue.
		/// </summary>
		private AsyncProducerConsumerQueue<NetworkIncomingMessage<TPayloadReadType>> IncomingMessageQueue { get; }

		//TODO: Do we need to syncronize these?
		private List<CancellationTokenSource> TaskTokenSources { get; }

		private PayloadInterceptionManager<TPayloadReadType> InterceptorManager { get; }

		/// <inheritdoc />
		public ManagedNetworkClient(TClientType unmanagedClient)
			: this(unmanagedClient, new NoOpLogger())
		{

		}

		public ManagedNetworkClient(TClientType unmanagedClient, ILog logger)
			: base(unmanagedClient, logger)
		{
			TaskTokenSources = new List<CancellationTokenSource>(2);
			OutgoingMessageQueue = new AsyncProducerConsumerQueue<TPayloadWriteType>(); //TODO: Should we constrain max count?
			IncomingMessageQueue = new AsyncProducerConsumerQueue<NetworkIncomingMessage<TPayloadReadType>>(); //TODO: Should we constrain max count?
			InterceptorManager = new PayloadInterceptionManager<TPayloadReadType>();
		}

		/// <inheritdoc />
		public override async Task<SendResult> SendMessage<TPayloadType>(TPayloadType payload, DeliveryMethod method)
		{
			if(payload == null) throw new ArgumentNullException(nameof(payload));

			//we just add the message to the queue and let the other threads/tasks deal with everything else.
			await OutgoingMessageQueue.EnqueueAsync(payload)
				.ConfigureAwait(false);

			return SendResult.Enqueued;
		}

		/// <inheritdoc />
		public override Task<NetworkIncomingMessage<TPayloadReadType>> ReadMessageAsync(CancellationToken token)
		{
			return IncomingMessageQueue.DequeueAsync(token);
		}

		private CancellationTokenSource CreateNewManagedCancellationTokenSource()
		{
			//Cretae and add to the token sources.
			CancellationTokenSource source = new CancellationTokenSource();
			TaskTokenSources.Add(source);
			return source;
		}

		/// <summary>
		/// Dispatches the outgoing messages scheduled to be send
		/// over the network.
		/// </summary>
		/// <returns>A future which will complete when the client disconnects.</returns>
		private async Task DispatchOutgoingMessages()
		{
			try
			{
				//We need a token for canceling this task when a user disconnects
				CancellationToken dispatchCancelation = CreateNewManagedCancellationTokenSource().Token;

				while(!dispatchCancelation.IsCancellationRequested)
				{
					TPayloadWriteType payload = await OutgoingMessageQueue.DequeueAsync(dispatchCancelation)
						.ConfigureAwait(false);

					await UnmanagedClient.WriteAsync(payload)
						.ConfigureAwait(false);
				}
					
			}
			catch(TaskCanceledException e)
			{
				//This is an expected exception that happens when the token is canceled
				if(Logger.IsDebugEnabled)
					Logger.Debug($"Expected Task Canceled Exception: {e.Message}\n\n Stack: {e.StackTrace}");

				//We cannot rethrow because this can cause application instability on threadpools
			}
			catch(Exception e)
			{
				if(Logger.IsErrorEnabled)
					Logger.Error($"Error: {e.Message}\n\n Stack: {e.StackTrace}");

				//We cannot rethrow because this can cause application instability on threadpools
			}
			finally
			{
				try
				{
					await DisconnectAsync(0);
				}
				catch(Exception)
				{

				}
			}

			//TODO: Should we do anything after the dispatch has stopped?
		}

		/// <summary>
		/// Reading the incoming messages from the network client and schedules them
		/// with the incoming message queue.
		/// </summary>
		/// <returns>A future which will complete when the client disconnects.</returns>
		private async Task EnqueueIncomingMessages()
		{
			//We need a token for canceling this task when a user disconnects
			CancellationToken incomingCancellationToken = CreateNewManagedCancellationTokenSource().Token;

			try
			{
				while(!incomingCancellationToken.IsCancellationRequested)
				{
					NetworkIncomingMessage<TPayloadReadType> message = await UnmanagedClient.ReadAsync(incomingCancellationToken)
						.ConfigureAwait(false);

					//If the message is null then the connection is no longer valid
					//The socket likely disconnected so we should stop the network thread
					if(message == null)
					{
						//We have to publish a null so it can be consumed by the user
						//to know that the socket is dead
						await IncomingMessageQueue.EnqueueAsync(null, CancellationToken.None)
							.ConfigureAwait(false);

						StopNetwork();
					}
						

					//if have to check the token again because the message may be null and may have been canceled mid-read
					if(incomingCancellationToken.IsCancellationRequested)
						continue;

					//Try to notify interceptors of a payload that has come in. They may want it
					if(!InterceptorManager.TryNotifyOutstandingInterceptors(message.Payload))
						await IncomingMessageQueue.EnqueueAsync(message, incomingCancellationToken)
							.ConfigureAwait(false);
				}
			}
			catch(TaskCanceledException e)
			{
				//This is an expected exception that happens when the token is canceled
				if(Logger.IsDebugEnabled)
					Logger.Debug($"Expected Task Canceled Exception: {e.Message}\n\n Stack: {e.StackTrace}");

				//We cannot rethrow because this can cause application instability on threadpools
			}
			catch(Exception e)
			{
				if(Logger.IsErrorEnabled)
					Logger.Error($"Error: {e.Message}\n\n Stack: {e.StackTrace}");

				//We cannot rethrow because this can cause application instability on threadpools
			}
			finally
			{
				try
				{
					await DisconnectAsync(0);
				}
				catch(Exception)
				{
					
				}
			}

			//TODO: Should we do anything after the dispatch has stopped?
		}

		/// <inheritdoc />
		public override async Task<bool> ConnectAsync(string address, int port)
		{
			bool result = await base.ConnectAsync(address, port);

			//TODO: We should remove this. We need to seperate connecting and starting the network handling.
			if(result)
				StartNetwork();

			return isConnected;
		}

		//TODO: Is it safe to make this method public? We made it public for the server stuff
		/// <summary>
		/// Stops the network tasks for the client.
		/// </summary>
		public void StopNetwork()
		{
			//Before disconnecting the managed client we should cancel all the tokens used for
			//running the tasks
			TaskTokenSources.ForEach(t =>
			{
				t.Cancel();

				//TODO: Is it safe not to dipose?
			});

			TaskTokenSources.Clear();
		}

		/// <inheritdoc />
		public override Task DisconnectAsync(int delay)
		{
			//Before disconnecting the managed client we should cancel all the tokens used for
			//running the tasks
			StopNetwork();

			return base.DisconnectAsync(delay);
		}

		/// <summary>
		/// Starts the network read and write queues.
		/// </summary>
		private void StartNetwork()
		{
			//TODO: Is it ok to not make these long-running?
			//Create both a read and write thread
			Task.Factory.StartNew(DispatchOutgoingMessages);
			Task.Factory.StartNew(EnqueueIncomingMessages);
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		public override void Dispose(bool disposing)
		{
			if(!disposedValue)
			{
				if(disposing)
				{
					// TODO: dispose managed state (managed objects).
					StopNetwork();
					//IncomingMessageQueue.Dispose();
					//OutgoingMessageQueue.Dispose();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~ManagedPSOBBNetworkClient() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion

		/// <inheritdoc />
		public override Task<TResponseType> InterceptPayload<TResponseType>(CancellationToken cancellationToken)
		{
			//Just dispatch to the manager
			return InterceptorManager.InterceptPayload<TResponseType>(cancellationToken);
		}
	}
}