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
	public class InternalClientSessionFactory : IClientSessionFactory
	{
		private AUIDServiceCollection<ClientSessionServiceContext> PeerServiceCollection { get; }

		/// <summary>
		/// Serializer capable of serializing outgoing messages of the designated format.
		/// </summary>
		private ISerializerStrategy Serializer { get; }

		private ILog PeerLogger { get; }

		private IAUIDService<INetPeer> NetPeerAUIDService { get; }

		private IManagedClientSessionFactory ManagedSessionFactory { get; }

		public InternalClientSessionFactory(AUIDServiceCollection<ClientSessionServiceContext> peerServiceCollection, ISerializerStrategy serializer, ILog peerLogger, IAUIDService<INetPeer> netPeerAuidService, IManagedClientSessionFactory managedSessionFactory)
		{
			if (peerServiceCollection == null) throw new ArgumentNullException(nameof(peerServiceCollection));
			if (serializer == null) throw new ArgumentNullException(nameof(serializer));
			if (peerLogger == null) throw new ArgumentNullException(nameof(peerLogger));
			if (netPeerAuidService == null) throw new ArgumentNullException(nameof(netPeerAuidService));
			if (managedSessionFactory == null) throw new ArgumentNullException(nameof(managedSessionFactory));

			PeerServiceCollection = peerServiceCollection;
			Serializer = serializer;
			PeerLogger = peerLogger;
			NetPeerAUIDService = netPeerAuidService;
			ManagedSessionFactory = managedSessionFactory;
		}

		/// <inheritdoc />
		public ClientPeerSession Create(IConnectionDetails connectionDetails, NetConnection connection)
		{
			if (connectionDetails == null) throw new ArgumentNullException(nameof(connectionDetails));
			if (connection == null) throw new ArgumentNullException(nameof(connection));

			//Build the message router service
			LidgrenNetworkMessageRouterService routerService = new LidgrenServerNetworkMessageRouterService(new LidgrenNetworkMessageFactory(), connection, Serializer);
			NetworkMessagePublisher basicMessagePublisher = new NetworkMessagePublisher();
			DefaultDisconnectionServiceHandler disconnectionHandler = new DefaultDisconnectionServiceHandler();

			//TODO: Clean this up
			disconnectionHandler.DisconnectionEventHandler += () => PeerServiceCollection.Remove(connectionDetails.ConnectionID);

			//Try to create the incoming peer; consumers of the library may reject the connection.
			ClientPeerSession session = ManagedSessionFactory.CreateIncomingPeerSession(routerService, connectionDetails, basicMessagePublisher, disconnectionHandler);

			if (session == null)
				return null;

			if (session.PeerDetails.ConnectionID == 0)
				throw new InvalidOperationException("Generated peer has an unset connection ID.");

			//Create a service context for the server.
			ClientSessionServiceContext serviceContext = new ClientSessionServiceContext(routerService, basicMessagePublisher, session);

			//Enter AUID lock
			PeerServiceCollection.syncObj.EnterWriteLock();
			try
			{
				PeerServiceCollection.Add(session.PeerDetails.ConnectionID, serviceContext);
			}
			finally
			{
				PeerServiceCollection.syncObj.ExitWriteLock();
			}

			return session;
		}
	}
}
