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
	public abstract class ManagedClientSession<TPayloadWriteType, TPayloadReadType> 
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
		/// The network address of the session.
		/// </summary>
		public NetworkAddressInfo ClientAddress { get; }

		/// <inheritdoc />
		protected ManagedClientSession(IManagedNetworkServerClient<TPayloadWriteType, TPayloadReadType> internalManagedNetworkClient, NetworkAddressInfo clientAddress)
		{
			if(internalManagedNetworkClient == null) throw new ArgumentNullException(nameof(internalManagedNetworkClient));
			if(clientAddress == null) throw new ArgumentNullException(nameof(clientAddress));

			InternalManagedNetworkClient = internalManagedNetworkClient;
			ClientAddress = clientAddress;
		}
	
		//TODO: We may also want to provide some message parameters when we add UDP support
		/// <summary>
		/// Called when a network message is recieved.
		/// Can also be called to simulate the recieveing a network message.
		/// </summary>
		/// <param name="message">The network message recieved.</param>
		/// <returns></returns>
		public abstract Task OnNetworkMessageRecieved(NetworkIncomingMessage<TPayloadReadType> message);
	}
}
