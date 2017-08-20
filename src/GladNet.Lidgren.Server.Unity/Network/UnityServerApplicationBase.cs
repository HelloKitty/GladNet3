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
using JetBrains.Annotations;

namespace GladNet.Lidgren.Server.Unity
{
	public abstract class UnityServerApplicationBase<TSerializationStrategy, TDeserializationStrategy, TSerializerRegistry> : MonoBehaviour, IClassLogger, IManagedClientSessionFactory , IApplicationBase
		where TSerializationStrategy : ISerializerStrategy, new() 
		where TDeserializationStrategy : IDeserializerStrategy, new() 
		where TSerializerRegistry : ISerializerRegistry, new()
	{
		//Contraining new() for generic type params in .Net 3.5 is very slow
		//This object should rarely be created. If in the future you must fix this slowness, which compiled to Activator, then
		//you should use a compliled lambda expression to create the object I think.

		public abstract ILog Logger { get; }

		private InternalUnityApplicationBase ManagedApplicationBase { get; set; }

		[SerializeField]
		private ConnectionInfo connectionInfo;

		public void StartServer()
		{
			NetPeerConfiguration config = new NetPeerConfiguration(connectionInfo.ApplicationIdentifier) { Port = connectionInfo.Port, AcceptIncomingConnections = true, UseMessageRecycling = true };

			//Server needs a much larger than default buffer size because corruption can happen otherwise
			config.ReceiveBufferSize = 500000;
			config.SendBufferSize = 500000;

			//Call the netconfig overload to start the server
			StartServer(config);
		}

		public abstract void RegisterTypes(ISerializerRegistry registry);

		public abstract ClientPeerSession CreateIncomingPeerSession(INetworkMessageRouterService sender, IConnectionDetails details, INetworkMessageSubscriptionService subService, IDisconnectionServiceHandler disconnectHandler, INetworkMessageRouteBackService routebackService);

		/// <summary>
		/// Called internally by Unity3D when the application is terminating.
		/// Overriders MUST call base.
		/// </summary>
		protected virtual void OnApplicationQuit()
		{
			//Disconnects and shutsdown server
			OnDestroy();
		}

		protected virtual void OnDestroy()
		{
			ManagedApplicationBase?.StopServer();
		}

		/// <inheritdoc />
		public void StartServer(NetPeerConfiguration netConfig)
		{
			if (ManagedApplicationBase != null)
				throw new InvalidOperationException("Server is already started.");

			ManagedApplicationBase = new InternalUnityApplicationBase(new TDeserializationStrategy(), new TSerializationStrategy(), Logger, this);

			//start the server
			ManagedApplicationBase.StartServer(netConfig);

			ISerializerRegistry serializerRegistry = new TSerializerRegistry();

			//First call the managed appbase's register
			ManagedApplicationBase.RegisterTypes(serializerRegistry);

			RegisterTypes(serializerRegistry);
		}

		/// <inheritdoc />
		public void StopServer()
		{
			//Just forward the stop to the internal app base
			ManagedApplicationBase?.StopServer();
		}
	}
}
