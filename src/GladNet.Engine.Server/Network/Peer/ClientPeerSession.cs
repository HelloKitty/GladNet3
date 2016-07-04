using GladNet.Common;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladNet.Payload;
using Easyception;
using GladNet.Engine.Common;
using GladNet.Message;

namespace GladNet.Engine.Server
{
	public abstract class ClientPeerSession : Peer, IClientSessionNetworkMessageSender, IClientSessionNetworkMessageRouter
	{
		private INetworkMessageRouteBackService messageRoutebackService { get; }

		public ClientPeerSession(ILog logger, INetworkMessageRouterService sender, IConnectionDetails details, INetworkMessageSubscriptionService subService,
			IDisconnectionServiceHandler disconnectHandler, INetworkMessageRouteBackService routebackService)
				: base(logger, sender, details, subService, disconnectHandler)
		{
			Throw<ArgumentNullException>.If.IsNull(subService)?.Now(nameof(subService));
			Throw<ArgumentNullException>.If.IsNull(routebackService)?.Now(nameof(routebackService));

			messageRoutebackService = routebackService;

			//Subscribe to request messages
			subService.SubscribeTo<RequestMessage>()
				.With(OnInternalReceiveRequest);
		}

		/// <summary>
		/// Indicates if a <see cref="Peer"/> can send the given <paramref name="opType"/>.
		/// </summary>
		/// <param name="opType"><see cref="OperationType"/> to check.</param>
		/// <returns>True if the <see cref="Peer"/> can send the given <see cref="OperationType"/>.</returns>
		public override bool CanSend(OperationType opType)
		{
			//Returns true if the opType matches ClientPeerSession opTypes AND the sender service can send the opType too.
			return opType == OperationType.Response || opType == OperationType.Event && NetworkSendService.CanSend(opType);
		}

		#region Message Senders
		/// <summary>
		/// Sends a networked response.
		/// </summary>
		/// <param name="payload"><see cref="PacketPayload"/> for the desired network response message.</param>
		/// <param name="deliveryMethod">Desired <see cref="DeliveryMethod"/> for the response. See documentation for more information.</param>
		/// <param name="encrypt">Optional: Indicates if the message should be encrypted. Default: false</param>
		/// <param name="channel">Optional: Inidicates the channel the network message should be sent on. Default: 0</param>
		/// <returns>Indication of the message send state.</returns>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public virtual SendResult SendResponse(PacketPayload payload, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			Throw<ArgumentNullException>.If.IsNull(payload)?.Now(nameof(payload));

			return NetworkSendService.TrySendMessage(OperationType.Response, payload, deliveryMethod, encrypt, channel);
		}

		/// <summary>
		/// Sends a networked event.
		/// </summary>
		/// <param name="payload"><see cref="PacketPayload"/> for the desired network event message.</param>
		/// <param name="deliveryMethod">Desired <see cref="DeliveryMethod"/> for the event. See documentation for more information.</param>
		/// <param name="encrypt">Optional: Indicates if the message should be encrypted. Default: false</param>
		/// <param name="channel">Optional: Inidicates the channel the network message should be sent on. Default: 0</param>
		/// <returns>Indication of the message send state.</returns>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public virtual SendResult SendEvent(PacketPayload payload, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			Throw<ArgumentNullException>.If.IsNull(payload)?.Now(nameof(payload));

			return NetworkSendService.TrySendMessage(OperationType.Event, payload, deliveryMethod, encrypt, channel);
		}

		/// <summary>
		/// Sends a networked event.
		/// Additionally this message/payloadtype is known to have static send parameters and those will be used in transit.
		/// </summary>
		/// <typeparam name="TPacketType">Type of the packet payload.</typeparam>
		/// <param name="payload">Payload instance to be sent in the message that contains static message parameters.</param>
		/// <returns>Indication of the message send state.</returns>
		public virtual SendResult SendEvent<TPacketType>(TPacketType payload) 
			where TPacketType : PacketPayload, IStaticPayloadParameters
		{
			Throw<ArgumentNullException>.If.IsNull(payload)?.Now(nameof(payload));

			return NetworkSendService.TrySendMessage<TPacketType>(OperationType.Event, payload);
		}

		/// <summary>
		/// Sends a networked response.
		/// Additionally this message/payloadtype is known to have static send parameters and those will be used in transit.
		/// </summary>
		/// <typeparam name="TPacketType">Type of the packet payload.</typeparam>
		/// <param name="payload">Payload instance to be sent in the message that contains static message parameters.</param>
		/// <returns>Indication of the message send state.</returns>
		public virtual SendResult SendResponse<TPacketType>(TPacketType payload) 
			where TPacketType : PacketPayload, IStaticPayloadParameters
		{
			Throw<ArgumentNullException>.If.IsNull(payload)?.Now(nameof(payload));

			return NetworkSendService.TrySendMessage<TPacketType>(OperationType.Response, payload);
		}
		#endregion

		/// <summary>
		/// Called internally first when a request is recieved from the remote peer.
		/// </summary>
		/// <param name="requestMessage"><see cref="IRequestMessage"/> sent by the peer.</param>
		/// <param name="parameters">Parameters the message was sent with.</param>
		private void OnInternalReceiveRequest(IRequestMessage requestMessage, IMessageParameters parameters)
		{
			//We should check if the message is routing back.
			//This is suggested in the GladNet2 routing specification
			//Under "Route-back Outside Userspace": https://github.com/HelloKitty/GladNet2.Specifications/blob/master/Routing/RoutingSpecification.md
			if (requestMessage.isRoutingBack)
			{
				messageRoutebackService.RouteRequest(requestMessage, parameters);
			}

			//GladNet2 routing specification dictates that we should push the AUID
			//into the routing stack:https://github.com/HelloKitty/GladNet2.Specifications/blob/master/Routing/RoutingSpecification.md
			requestMessage.Push(PeerDetails.ConnectionID);

			OnReceiveRequest(requestMessage, parameters);
		}

		/// <summary>
		/// Called internally second when a request is recieved from the remote peer.
		/// </summary>
		/// <param name="requestMessage"><see cref="IRequestMessage"/> sent by the peer.</param>
		/// <param name="parameters">Parameters the message was sent with.</param>
		protected abstract void OnReceiveRequest(IRequestMessage requestMessage, IMessageParameters parameters);

		/// <summary>
		/// Routes a <see cref="IResponseMessage"/> message.
		/// </summary>
		/// <param name="message"><see cref="IResponseMessage"/> to route.</param>
		/// <param name="deliveryMethod">Desired <see cref="DeliveryMethod"/> for the response. See documentation for more information.</param>
		/// <param name="encrypt">Optional: Indicates if the message should be encrypted. Default: false</param>
		/// <param name="channel">Optional: Inidicates the channel the network message should be sent on. Default: 0</param>
		/// <returns>Indication of the message send state.</returns>
		public SendResult RouteResponse(IResponseMessage message, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			Throw<ArgumentNullException>.If.IsNull(message)?.Now(nameof(message));

			return NetworkSendService.TryRouteMessage(message, deliveryMethod, encrypt, channel);
		}
	}
}
