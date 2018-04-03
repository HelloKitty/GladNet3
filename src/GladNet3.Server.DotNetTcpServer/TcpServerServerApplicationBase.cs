using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GladNet;

namespace GladNet
{
	//TODO: Add logging
	public abstract class TcpServerServerApplicationBase<TPayloadWriteType, TPayloadReadType> 
		where TPayloadWriteType : class 
		where TPayloadReadType : class
	{
		/// <summary>
		/// Network address information for the server.
		/// </summary>
		public NetworkAddressInfo ServerAddress { get; }

		/// <summary>
		/// The internally managed <see cref="TcpListener"/>
		/// </summary>
		private Lazy<TcpListener> ManagedTcpServer { get; }

		//TODO: Why would child want access to this? Make case for protected
		private INetworkMessageDispatchingStrategy<TPayloadWriteType, TPayloadReadType> MessageHandlingStrategy { get; }

		protected TcpServerServerApplicationBase(NetworkAddressInfo serverAddress)
			: this(serverAddress, new InPlaceNetworkMessageDispatchingStrategy<TPayloadWriteType, TPayloadReadType>())
		{

		}

		protected TcpServerServerApplicationBase(NetworkAddressInfo serverAddress, INetworkMessageDispatchingStrategy<TPayloadWriteType, TPayloadReadType> messageHandlingStrategy)
		{
			if(serverAddress == null) throw new ArgumentNullException(nameof(serverAddress));
			if(messageHandlingStrategy == null) throw new ArgumentNullException(nameof(messageHandlingStrategy));

			ServerAddress = serverAddress;
			ManagedTcpServer = new Lazy<TcpListener>(CreateTcpListener, true);
			MessageHandlingStrategy = messageHandlingStrategy;
		}

		/// <summary>
		/// Starts the internal TCP Server.
		/// You should call <see cref="BeginListening"/> after this
		/// or nothing will happen.
		/// </summary>
		/// <returns></returns>
		public bool StartServer()
		{
			ManagedTcpServer.Value.Start();

			//TODO: Do we need to return anything?
			return true;
		}

		public async Task BeginListening()
		{
			//TODO: We should create a way to stop and throw if not started
			while(true)
			{

				TcpClient client = await ManagedTcpServer.Value.AcceptTcpClientAsync()
					.ConfigureAwait(false);

				//TODO: Add some info/debug logging
				//We should ask the implementer if this client should be accepted
				//it is possible they do not want to accept this client for a number
				//of potential reasons
				if(!IsClientAcceptable(client))
				{
					//TODO: Is this how we should handle?
					client.Client.Shutdown(SocketShutdown.Both);
					client.GetStream().Dispose();
					client.Dispose();
					continue;
				}

				var internalNetworkClient = CreateIncomingSessionPipeline(client);
				var networkSession = CreateIncomingSession(internalNetworkClient);

				//Don't allow invalid or null clients to proceed
				//If implementers don't want to create a client they shouldn't return null
				//and instead should return false from IsClientAcceptable
				if(internalNetworkClient == null)
					throw new InvalidOperationException($"Created an invalid client from {nameof(CreateIncomingSessionPipeline)}.");

				if(networkSession == null)
					throw new InvalidOperationException($"Created an invalid client from {nameof(CreateIncomingSession)}.");

				//TODO: We may not make this public for long. There should be a better way.
				//networkClient.StartNetwork();

				//TODO: Refactor
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
				Task.Run(async () =>
				{
					while(internalNetworkClient.isConnected)
					{
						NetworkIncomingMessage<TPayloadReadType> message = await internalNetworkClient.ReadMessageAsync(CancellationToken.None);

						//TODO: This will work for World of Warcraft since it requires no more than one packet
						//from the same client be handled at one time. However it limits throughput and maybe we should
						//handle this at a different level instead. 
						await HandleIncomingNetworkMessage(networkSession, message);
					}
				})
					.ConfigureAwait(false);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			}
		}

		//TODO: Should we force implementers to create handling logic?
		/// <summary>
		/// Implementers can override this virtual handling method to specify the strategy for how
		/// messages should be handled. The default implementation is to handle them in-place asyncrounously on the context
		/// that they were recieved in.
		/// </summary>
		/// <param name="session">The network session that sent the message.</param>
		/// <param name="message">The incoming message.</param>
		/// <returns>An awaitable that finishes when the session can continue handling messages.</returns>
		protected async Task HandleIncomingNetworkMessage(ManagedClientSession<TPayloadWriteType, TPayloadReadType> session, NetworkIncomingMessage<TPayloadReadType> message)
		{
			if(session == null) throw new ArgumentNullException(nameof(session));
			if(message == null) throw new ArgumentNullException(nameof(message));

			await MessageHandlingStrategy.DispatchNetworkMessage(new SessionMessageContext<TPayloadWriteType, TPayloadReadType>(session, message));
		}

		/// <summary>
		/// Indicates if the provided <see cref="TcpClient"/> is acceptable.
		/// Return true if the client should be handled. This will likely lead to
		/// a call to <see cref="CreateIncomingSessionPipeline"/>. Returning false
		/// will mean this connection should be disconnected and rejected and no
		/// client representation will be created for it.
		/// </summary>
		/// <param name="tcpClient">The <see cref="TcpClient"/> to check the acceptance for.</param>
		/// <returns></returns>
		protected abstract bool IsClientAcceptable(TcpClient tcpClient);

		/// <summary>
		/// Called internally when a connection is recieved.
		/// The parameter provided is the <see cref="TcpClient"/> representation of the connection.
		/// </summary>
		/// <param name="client">The <see cref="TcpClient"/> associated with the incoming connection.</param>
		/// <returns>A non-null client.</returns>
		protected abstract IManagedNetworkServerClient<TPayloadWriteType, TPayloadReadType> CreateIncomingSessionPipeline(TcpClient client);

		/// <summary>
		/// Called internally when a connection's session pipeline has been created.
		/// The parameter provided is gernally built using <see cref="CreateIncomingSessionPipeline"/>.
		/// This method should produce a valid session and is considered the hub of the connection.
		/// </summary>
		/// <param name="client">The managed client.</param>
		/// <returns>A non-null session.</returns>
		protected abstract ManagedClientSession<TPayloadWriteType, TPayloadReadType> CreateIncomingSession(IManagedNetworkServerClient<TPayloadWriteType, TPayloadReadType> client);

		/// <summary>
		/// Called internally to create the server's <see cref="TcpListener"/>.
		/// Must only be called once. Will throw if called multiple times.
		/// Expects <see cref="ServerAddress"/> to be properly initialized.
		/// </summary>
		/// <returns>A valid <see cref="TcpListener"/> at started with the information of <see cref="ServerAddress"/>.</returns>
		private TcpListener CreateTcpListener()
		{
			if(ServerAddress?.AddressEndpoint == null)
				throw new InvalidOperationException($"Failed to start server. {nameof(ServerAddress)} property must be initialized properly.");

			if(ManagedTcpServer.IsValueCreated)
				throw new InvalidOperationException($"Tried to start {nameof(ManagedTcpServer)} multiple times.");

			return new TcpListener(ServerAddress.AddressEndpoint, ServerAddress.Port);
		}
	}
}
