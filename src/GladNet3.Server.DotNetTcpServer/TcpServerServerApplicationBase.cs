using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GladNet;

namespace GladNet3
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

		protected TcpServerServerApplicationBase(NetworkAddressInfo serverAddress)
		{
			if(serverAddress == null) throw new ArgumentNullException(nameof(serverAddress));

			ServerAddress = serverAddress;
			ManagedTcpServer = new Lazy<TcpListener>(CreateTcpListener, true);
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

				TcpClient client = await ManagedTcpServer.Value.AcceptTcpClientAsync();

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
				
				var networkClient = CreateIncomingClientPipeline(client);

				//Don't allow invalid or null clients to proceed
				//If implementers don't want to create a client they shouldn't return null
				//and instead should return false from IsClientAcceptable
				if(networkClient == null)
					throw new InvalidOperationException($"Created an invalid client from {nameof(CreateIncomingClientPipeline)}.");

				//TODO: We may not make this public for long. There should be a better way.
				networkClient.StartNetwork();

				//TODO: Refactor
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
				Task.Factory.StartNew(async () =>
				{
					while(networkClient.isConnected)
					{
						NetworkIncomingMessage<TPayloadReadType> message = await networkClient.ReadMessageAsync(CancellationToken.None);

						await HandleIncomingNetworkMessage(networkClient, message);
					}
				}, TaskCreationOptions.LongRunning);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			}
		}

		//TODO: Should we force implementers to create handling logic?
		/// <summary>
		/// Implementers should handle the message handling.
		/// This method will be called providing the network messaged recieved and the
		/// associated <see cref="TcpClient"/> that sent it. Implementers are left to
		/// create handling logic. Recommending the GladNet handler API.
		/// </summary>
		/// <param name="networkClient">The network client that sent the message.</param>
		/// <param name="message">The incoming message.</param>
		/// <returns>An awaitable that finishes when the message has been fully handled.</returns>
		protected abstract Task HandleIncomingNetworkMessage(IManagedNetworkClient<TPayloadWriteType, TPayloadReadType> networkClient, NetworkIncomingMessage<TPayloadReadType> message);

		/// <summary>
		/// Indicates if the provided <see cref="TcpClient"/> is acceptable.
		/// Return true if the client should be handled. This will likely lead to
		/// a call to <see cref="CreateIncomingClientPipeline"/>. Returning false
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
		/// <param name="client"></param>
		/// <returns></returns>
		protected abstract IManagedNetworkClient<TPayloadWriteType, TPayloadReadType> CreateIncomingClientPipeline(TcpClient client);

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
