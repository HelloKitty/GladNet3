using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GladNet;

namespace GladNet
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TPayloadWriteType"></typeparam>
	/// <typeparam name="TPayloadReadType"></typeparam>
	public abstract class ManagedClientSession<TPayloadWriteType, TPayloadReadType>  : IManagedClientSession, INetworkMessageReceivable<TPayloadReadType>
		where TPayloadWriteType : class 
		where TPayloadReadType : class
	{
		/// <summary>
		/// Internal syncronization object.
		/// </summary>
		private readonly object syncObj = new object();

		//This has to be internal because it needs to be access at the library level.
		private IManagedNetworkServerClient<TPayloadWriteType, TPayloadReadType> InternalManagedNetworkClient { get; }

		//We want to expose the below services but not ALL the services a ManagedNetworkClient provides
		/// <summary>
		/// 
		/// </summary>
		public IPeerPayloadSendService<TPayloadWriteType> SendService => InternalManagedNetworkClient;

		//TODO: We should create an event that can be subscribed to for disconnecting
		//TODO: Is this the best way to do this?
		/// <summary>
		/// Service that can be used for disconnecting the session.
		/// </summary>
		public IConnectionService Connection => InternalManagedNetworkClient;

		/// <summary>
		/// The details of the session.
		/// </summary>
		public SessionDetails Details { get; }

		public event StatusChangeEvent OnSessionDisconnection;

		/// <summary>
		/// Indicates if a disconnection has already been called.
		/// We don't want to handle disconnection multiple times.
		/// </summary>
		private bool HasDisconnectedBeenCalled { get; set; } = false;

		/// <inheritdoc />
		protected ManagedClientSession(IManagedNetworkServerClient<TPayloadWriteType, TPayloadReadType> internalManagedNetworkClient, SessionDetails details)
		{
			if(internalManagedNetworkClient == null) throw new ArgumentNullException(nameof(internalManagedNetworkClient));
			if(details == null) throw new ArgumentNullException(nameof(details));

			InternalManagedNetworkClient = internalManagedNetworkClient;
			Details = details;
		}

		/// <summary>
		/// Disconnects the session and invokes the
		/// <see cref="OnSessionDisconnection"/> event.
		/// </summary>
		public async Task DisconnectClientSession()
		{
			lock(syncObj)
			{
				//This prevents us from calling disconnected multiple times.
				if(HasDisconnectedBeenCalled)
					return;

				HasDisconnectedBeenCalled = true;
			}

			if(OnSessionDisconnection != null)
				await OnSessionDisconnection.Invoke(this, new DisconnectedSessionStatusChangeEventArgs(Details))
					.ConfigureAwait(false);

			OnSessionDisconnection = null;

			//TODO: Can this ever throw??
			//Also disconnect the network client
			await InternalManagedNetworkClient.DisconnectAsync(0)
				.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public abstract Task OnNetworkMessageRecieved(NetworkIncomingMessage<TPayloadReadType> message);
	}
}
