using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using GladNet.Engine.Common;
using GladNet.Engine.Server;
using GladNet.Lidgren.Engine.Common;
using GladNet.Serializer;
using Lidgren.Network;

namespace GladNet.Lidgren.Server
{
	public abstract class ApplicationBase : IClassLogger, ILidgrenMessagePollable, ILidgrenMessageDispatcher, IApplicationBase
	{
		private ManagedLidgrenNetworkThread managedNetworkThread { get; set; }

		private NetServer internalLidgrenServer { get; set; }

		private SessionlessMessageHandler sessionlessHandler { get; }

		private AUIDServiceCollection<ClientSessionServiceContext> peerServiceCollection { get; }

		private IAUIDService<INetPeer> netPeerAUIDService { get; }

		/// <inheritdoc />
		public ILog Logger { get; }

		protected ApplicationBase(IDeserializerStrategy deserializer, ISerializerStrategy serializer, ILog logger, IManagedClientSessionFactory sessionManagedFactory)
		{
			if (deserializer == null) throw new ArgumentNullException(nameof(deserializer));
			if (serializer == null) throw new ArgumentNullException(nameof(serializer));
			if (logger == null) throw new ArgumentNullException(nameof(logger));

			Logger = logger;

			//Init internal services/components
			//Create these first; thread needs them
			peerServiceCollection = new AUIDServiceCollection<ClientSessionServiceContext>(50);
			netPeerAUIDService = new AUIDNetPeerServiceDecorator(peerServiceCollection);

			managedNetworkThread = new ManagedLidgrenNetworkThread(serializer, new LidgrenServerMessageContextFactory(deserializer), new PeerSendServiceSelectionStrategy(peerServiceCollection), e => Logger.Fatal($"{e.Message} StackTrace: {e.StackTrace}"));
			InternalClientSessionFactory sessionFactory = new InternalClientSessionFactory(peerServiceCollection, serializer, Logger, netPeerAUIDService, sessionManagedFactory);
			sessionlessHandler = new SessionlessMessageHandler(sessionFactory, Logger);
		}

		public void StartServer(NetPeerConfiguration netConfig)
		{
			if (netConfig == null) throw new ArgumentNullException(nameof(netConfig));

			//Server needs a much larger than default buffer size because corruption can happen otherwise
			//config.ReceiveBufferSize = 500000;
			//config.SendBufferSize = 500000;

			internalLidgrenServer = new NetServer(netConfig);

			internalLidgrenServer.Start();

			//Do not forget to start network thread
			managedNetworkThread.Start(internalLidgrenServer);
		}

		/// <summary>
		/// Called internally when the server application is stopping.
		/// </summary>
		protected abstract void OnServerStop();

		public void StopServer()
		{
			//Disconnects and shutsdown server
			internalLidgrenServer?.Shutdown("Server shutdown");
			managedNetworkThread?.Stop();
			managedNetworkThread?.Dispose();
			managedNetworkThread = null;
			internalLidgrenServer = null;

			OnServerStop();
		}

		/// <inheritdoc />
		public abstract void RegisterTypes(ISerializerRegistry registry);

		public LidgrenMessageContext[] Poll()
		{
			if (managedNetworkThread == null)
				throw new InvalidOperationException("No network thread is running.");

			if (internalLidgrenServer.Status == NetPeerStatus.NotRunning)
				throw new InvalidOperationException("The server is not running.");

			//TODO: Maybe read to check count first.
			//Lock only as short as we need to.
			managedNetworkThread.IncomingMessageQueue.SyncRoot.EnterWriteLock();
			try
			{
				return managedNetworkThread.IncomingMessageQueue.DequeueAll()?.ToArray();
			}
			finally
			{
				managedNetworkThread.IncomingMessageQueue.SyncRoot.ExitWriteLock();
			}
		}

		public void DispatchMessages(IEnumerable<LidgrenMessageContext> messages)
		{
			if (messages == null) throw new ArgumentNullException(nameof(messages));

			//We have to check for messages that don't have an available peer.
			foreach (LidgrenMessageContext message in messages)
			{
				if (!peerServiceCollection.ContainsKey(message.ConnectionId)) //If it's unconnected we'll likely recieve a Connect/Establish event and the connection ID will be assigned at that point
					sessionlessHandler.HandleMessage(message);
				else
					message.TryDispatch(peerServiceCollection[message.ConnectionId].MessageReceiver); //TODO: Checking

				//TODO: Logging on no handler
			}
		}
	}
}
