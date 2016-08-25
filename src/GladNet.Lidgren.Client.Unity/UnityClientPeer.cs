using GladNet.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GladNet.Common;
using Lidgren.Network;
using GladNet.Serializer;
using GladNet.Lidgren.Engine.Common;
using System.Net;
using GladNet.Message;
using GladNet.Payload;
using System.Diagnostics.CodeAnalysis;

namespace GladNet.Lidgren.Client.Unity
{
	public abstract class UnityClientPeer<TSerializationStrategy, TDeserializationStrategy, TSerializerRegistry> : MonoBehaviour, INetPeer, IClientPeerNetworkMessageRouter, IClientPeerPayloadSender, IClientNetworkMessageReciever
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

		[SerializeField]
		private ConnectionInfo connectionInfo;

		public INetworkMessageRouterService NetworkSendService { get; private set; }

		public IConnectionDetails PeerDetails { get; private set; }

		public NetStatus Status { get; private set; } = NetStatus.Disconnected; //initial state should be disconnected

		private NetClient internalLidgrenNetworkClient { get; set; }

		private ManagedLidgrenNetworkThread managedNetworkThread { get; set; }

		private NetworkMessagePublisher publisher { get; } = new NetworkMessagePublisher();

		public void Awake()
		{
			//Initialize basic services
			internalLidgrenNetworkClient = new NetClient(new NetPeerConfiguration(connectionInfo.ApplicationIdentifier) { AcceptIncomingConnections = false });
			PeerDetails = new LidgrenConnectionDetailsAdapter(connectionInfo.ServerIp, connectionInfo.RemotePort, 0, 0); //we don't know port and id is not important on client
			RegisterPayloadTypes(this.serializerRegister);

			//Subscribe to the messages.
			publisher.SubscribeTo<EventMessage>()
				.With(new OnNetworkEventMessage(this.OnReceiveEvent));

			publisher.SubscribeTo<ResponseMessage>()
				.With(new OnNetworkResponseMessage(this.OnReceiveResponse));

			publisher.SubscribeTo<StatusMessage>()
				.With(new OnNetworkStatusMessage( (m, p) => this.OnStatusChanged(m.Status)));

			//Register the payloads
			RegisterPayloadTypes(serializerRegister);
		}

		public abstract void RegisterPayloadTypes(ISerializerRegistry registry);

		public bool CanSend(OperationType opType)
		{
			//Clients can only send responses
			return opType == OperationType.Request;
		}

		public void Disconnect()
		{
			//Clean up thread
			managedNetworkThread.Stop();
			managedNetworkThread.Dispose();
			managedNetworkThread = null;

			//Reinit
			internalLidgrenNetworkClient = new NetClient(new NetPeerConfiguration(connectionInfo.ApplicationIdentifier) { AcceptIncomingConnections = false });
			//NetworkSendService = new LidgrenClientNetworkMessageRouterService(new LidgrenNetworkMessageFactory(), internalLidgrenNetworkClient, serializer);
		}

		public bool Connect()
		{
			if (internalLidgrenNetworkClient.Status != NetPeerStatus.NotRunning)
			{
				//call disconnection to cleanup the current running session
				Disconnect();
			}

			//Must call start first
			internalLidgrenNetworkClient.Start();

			NetConnection connection = internalLidgrenNetworkClient.Connect(connectionInfo.ServerIp, connectionInfo.RemotePort);

			if (connection == null)
				throw new InvalidOperationException($"Could not connect and create a {nameof(NetConnection)}.");

			//Now that we have the netconnection we can initialize the sendservice
			NetworkSendService = new LidgrenClientNetworkMessageRouterService(new LidgrenNetworkMessageFactory(), connection, serializer);

			if (connection == null)
				return false;

			//Create a new managed thread
			managedNetworkThread = new ManagedLidgrenNetworkThread(serializer, new LidgrenClientMessageContextFactory(deserializer), new ClientSendServiceSelectionStrategy(this.NetworkSendService), e => Debug.LogError(e.Message + "StackTrace: " + e.StackTrace));

			//Start the thread and give the peer the context
			managedNetworkThread.Start(internalLidgrenNetworkClient);

			return true;
		}

		public void Poll()
		{
			if (managedNetworkThread == null)
				throw new InvalidOperationException("No network thread is running.");

			if (internalLidgrenNetworkClient.Status == NetPeerStatus.NotRunning)
				throw new InvalidOperationException("The client is not running.");


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
			foreach (LidgrenMessageContext m in messages)
				if (!m.TryDispatch(publisher))
					throw new InvalidOperationException($"Unable to dispatch {m.GetType().Name}.");
		}

		/// <summary>
		/// Called internally by Unity3D when the application is terminating.
		/// Overriders MUST call base.
		/// </summary>
		protected virtual void OnApplicationQuit()
		{
			if (managedNetworkThread != null)
			{
				//Clean up thread
				managedNetworkThread.Stop();
				managedNetworkThread.Dispose();
				managedNetworkThread = null;
			}

			//Disconnect if connected
			if (internalLidgrenNetworkClient.ConnectionStatus == NetConnectionStatus.Connected)
				internalLidgrenNetworkClient.Disconnect("");
		}

		/// <summary>
		/// Sends a networked request.
		/// </summary>
		/// <param name="payload"><see cref="PacketPayload"/> for the desired network request message.</param>
		/// <param name="deliveryMethod">Desired <see cref="DeliveryMethod"/> for the request. See documentation for more information.</param>
		/// <param name="encrypt">Optional: Indicates if the message should be encrypted. Default: false</param>
		/// <param name="channel">Optional: Inidicates the channel the network message should be sent on. Default: 0</param>
		/// <returns>Indication of the message send state.</returns>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public SendResult SendRequest(PacketPayload payload, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			return NetworkSendService.TrySendMessage(OperationType.Request, payload, deliveryMethod, encrypt, channel);
		}

		/// <summary>
		/// Sends a networked request.
		/// Additionally this message/payloadtype is known to have static send parameters and those will be used in transit.
		/// </summary>
		/// <typeparam name="TPacketType">Type of the packet payload.</typeparam>
		/// <param name="payload">Payload instance to be sent in the message that contains static message parameters.</param>
		/// <returns>Indication of the message send state.</returns>
		public SendResult SendRequest<TPacketType>(TPacketType payload)
			where TPacketType : PacketPayload, IStaticPayloadParameters
		{
			return NetworkSendService.TrySendMessage(OperationType.Request, payload);
		}

		public SendResult RouteRequest(IRequestMessage message, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			return NetworkSendService.TryRouteMessage(message, deliveryMethod, encrypt, channel);
		}

		/// <summary>
		/// Handles a <see cref="PacketPayload"/> sent as a response.
		/// </summary>
		/// <param name="payload">Response payload data from the network.</param>
		public abstract void OnReceiveResponse(IResponseMessage message, IMessageParameters parameters);

		/// <summary>
		/// Handles a <see cref="PacketPayload"/> sent as an event.
		/// </summary>
		/// <param name="payload">Event payload data from the network.</param>
		public abstract void OnReceiveEvent(IEventMessage message, IMessageParameters parameters);

		/// <summary>
		/// Handles a changed <see cref="NetStatus"/> stat from either local events or network events.
		/// </summary>
		/// <param name="status">Current status.</param>
		public abstract void OnStatusChanged(NetStatus status);
	}
}
