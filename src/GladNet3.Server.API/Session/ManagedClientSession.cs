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
	public abstract class ManagedClientSession<TPayloadWriteType, TPayloadReadType>  : IManagedClientSession
		where TPayloadWriteType : class 
		where TPayloadReadType : class
	{
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
	
		//TODO: We may also want to provide some message parameters when we add UDP support
		/// <summary>
		/// Called when a network message is recieved.
		/// Can also be called to simulate the recieveing a network message.
		/// </summary>
		/// <param name="message">The network message recieved.</param>
		/// <returns></returns>
		public abstract Task OnNetworkMessageRecieved(NetworkIncomingMessage<TPayloadReadType> message);

		/// <summary>
		/// Invoked internally when the session disconnects.
		/// </summary>
		protected abstract void OnSessionDisconnected();

		/// <summary>
		/// Disconnects the session and invokes the
		/// <see cref="OnSessionDisconnection"/> event.
		/// </summary>
		public void DisconnectClientSession()
		{
			//This prevents us from calling disconnected multiple times.
			if(HasDisconnectedBeenCalled)
				return;

			HasDisconnectedBeenCalled = true;
			OnSessionDisconnection?.Invoke(this, new DisconnectedSessionStatusChangeEventArgs(Details));
			OnSessionDisconnection = null;
			OnSessionDisconnected();
		}
	}
}
