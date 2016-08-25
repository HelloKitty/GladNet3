using GladNet.Engine.Server;
using GladNet.Lidgren.Engine.Common;
using GladNet.Serializer;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GladNet.Engine.Common;
using Common.Logging;

namespace GladNet.Lidgren.Server.Unity
{
	public abstract class UnityServerApplicationBase<TSerializationStrategy, TDeserializationStrategy, TSerializerRegistry> : MonoBehaviour, IClientSessionFactory, IClassLogger
		where TSerializationStrategy : ISerializerStrategy, new() where TDeserializationStrategy : IDeserializerStrategy, new() where TSerializerRegistry : ISerializerRegistry, new()
	{
		//Contraining new() for generic type params in .Net 3.5 is very slow
		//This object should rarely be created. If in the future you must fix this slowness, which compiled to Activator, then
		//you should use a compliled lambda expression to create the object I think.

		/// <summary>
		/// Deserializer capable of deserializing incoming messages of the expected format.
		/// </summary>
		private IDeserializerStrategy deserializer { get; } = new TDeserializationStrategy();

		/// <summary>
		/// Serializer capable of serializing outgoing messages of the designated format.
		/// </summary>
		private ISerializerStrategy serializer { get; } = new TSerializationStrategy();

		//Dont assume calls to this service register types for all serializers.
		//Though that probably is the case.
		/// <summary>
		/// Serialization registry service that provides simple type registeration services to make aware specified types
		/// to the serializer service called <see cref="serializer"/> within this class.
		/// </summary>
		private ISerializerRegistry serializerRegister { get; } = new TSerializerRegistry();

		private ManagedLidgrenNetworkThread managedNetworkThread { get; set; }

		private NetServer internalLidgrenServer { get; set; }

		private SessionlessMessageHandler sessionlessHandler { get; set; }

		private AUIDServiceCollection<ClientSessionServiceContext> peerServiceCollection { get; set; }

		private IAUIDService<INetPeer> netPeerAUIDService { get; set; }

		public abstract ILog Logger { get; }

		[SerializeField]
		private ConnectionInfo connectionInfo;

		public void StartServer()
		{
			NetPeerConfiguration config = new NetPeerConfiguration(connectionInfo.ApplicationIdentifier) { Port = connectionInfo.Port, AcceptIncomingConnections = true, UseMessageRecycling = true };

			//Server needs a much larger than default buffer size because corruption can happen otherwise
			config.ReceiveBufferSize = 500000;
			config.SendBufferSize = 500000;

			internalLidgrenServer = new NetServer(config);

			internalLidgrenServer.Start();

			//Register the server payloads
			RegisterPayloadTypes(this.serializerRegister);

			sessionlessHandler = new SessionlessMessageHandler(this);

			//Create these first; thread needs them
			peerServiceCollection = new AUIDServiceCollection<ClientSessionServiceContext>(50);
			netPeerAUIDService = new AUIDNetPeerServiceDecorator(peerServiceCollection);

			managedNetworkThread = new ManagedLidgrenNetworkThread(serializer, new LidgrenServerMessageContextFactory(deserializer), new PeerSendServiceSelectionStrategy(peerServiceCollection));
		}

		public abstract void RegisterPayloadTypes(ISerializerRegistry registry);

		public void Poll()
		{
			if (managedNetworkThread == null)
				throw new InvalidOperationException("No network thread is running.");

			if (internalLidgrenServer.Status == NetPeerStatus.NotRunning)
				throw new InvalidOperationException("The server is not running.");

			IEnumerable<LidgrenMessageContext> messages = null;

			//TODO: Maybe read to check count first.
			//Lock only as short as we need to.
			managedNetworkThread.IncomingMessageQueue.syncRoot.EnterWriteLock();
			try
			{
				messages = managedNetworkThread.IncomingMessageQueue.DequeueAll();
			}
			finally
			{
				managedNetworkThread.IncomingMessageQueue.syncRoot.ExitWriteLock();
			}

			if (messages == null || messages.Count() == 0)
				return;

			HandleMessages(messages);
		}

		private void HandleMessages(IEnumerable<LidgrenMessageContext> messages)
		{
			//We have to check for messages that don't have an available peer.
			foreach(LidgrenMessageContext message in messages)
			{
				if (message.ConnectionId == 0) //If connection ID is 0 then it's a message without a session
					sessionlessHandler.HandleMessage(message);
				else
					if (peerServiceCollection.ContainsKey(message.ConnectionId))
						message.TryDispatch(peerServiceCollection[message.ConnectionId].MessageReceiver); //TODO: Checking

				//TODO: Logging on no handler		
			}
		}

		protected abstract ClientPeerSession CreateIncomingPeerSession(INetworkMessageRouterService sender, IConnectionDetails details, INetworkMessageSubscriptionService subService,
			IDisconnectionServiceHandler disconnectHandler, INetworkMessageRouteBackService routebackService);

		//TODO: Move this to a factory
		public ClientPeerSession Create(IConnectionDetails connectionDetails, NetConnection connection)
		{
			//Build the message router service
			LidgrenNetworkMessageRouterService routerService = new LidgrenServerNetworkMessageRouterService(new LidgrenNetworkMessageFactory(), connection, serializer);
			NetworkMessagePublisher basicMessagePublisher = new NetworkMessagePublisher();
			DefaultNetworkMessageRouteBackService routebackService = new DefaultNetworkMessageRouteBackService(this.netPeerAUIDService, Logger);
			DefaultDisconnectionServiceHandler disconnectionHandler = new DefaultDisconnectionServiceHandler();

			//TODO: Clean this up
			disconnectionHandler.DisconnectionEventHandler += () => peerServiceCollection.Remove(connectionDetails.ConnectionID);

			//Try to create the incoming peer; consumers of the library may reject the connection.
			ClientPeerSession session = CreateIncomingPeerSession(routerService, connectionDetails, basicMessagePublisher, null, routebackService);

			if (session == null)
				return null;

			if (session.PeerDetails.ConnectionID == 0)
				throw new InvalidOperationException("Generated peer has an unset connection ID.");

			//Create a service context for the server.
			ClientSessionServiceContext serviceContext = new ClientSessionServiceContext(routerService, basicMessagePublisher, session);

			//Enter AUID lock
			this.peerServiceCollection.syncObj.EnterWriteLock();
			try
			{
				this.peerServiceCollection.Add(session.PeerDetails.ConnectionID, serviceContext);
			}
			finally
			{
				this.peerServiceCollection.syncObj.ExitWriteLock();
			}

			return session;
		}
	}
}
