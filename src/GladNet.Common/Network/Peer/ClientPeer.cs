using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using System.Diagnostics.CodeAnalysis;
using GladNet.Payload;
using Easyception;

namespace GladNet.Common
{
	public abstract class ClientPeer : Peer, IClientPeerPayloadSender
	{
#if !ENDUSER
		private INetworkMessageRouteBackService messageRoutebackService { get; }
#endif

		protected ClientPeer(ILog logger, INetworkMessageRouterService messageSender, IConnectionDetails details, INetworkMessageSubscriptionService subService,
			IDisconnectionServiceHandler disconnectHandler
#if !ENDUSER
			, INetworkMessageRouteBackService routebackService)
#else
			)
#endif
				: base(logger, messageSender, details, subService, disconnectHandler)
		{
			Throw<ArgumentNullException>.If.IsNull(subService)?.Now(nameof(subService));

//Enduser clients shouldn't be routing messages so we don't need to call internal method
//Example: A gameclient for a player. These sorts of clients do NOT need to route messages they recieve.
#if !ENDUSER
			//ClientPeers should be interested in events and responses from the server they are a peer of
			subService.SubscribeTo<EventMessage>()
				.With(OnReceiveEvent);

			subService.SubscribeTo<ResponseMessage>()
				.With(OnInternalReceiveResponse);

			Throw<ArgumentNullException>.If.IsNull(routebackService)?.Now(nameof(routebackService));
			messageRoutebackService = routebackService;
#else
			//ClientPeers should be interested in events and responses from the server they are a peer of
			subService.SubscribeTo<EventMessage>()
				.With(OnReceiveEvent);

			subService.SubscribeTo<ResponseMessage>()
				.With(OnReceiveResponse);
#endif
		}

		/// <summary>
		/// Indicates if a <see cref="Peer"/> can send the given <paramref name="opType"/>.
		/// </summary>
		/// <param name="opType"><see cref="OperationType"/> to check.</param>
		/// <returns>True if the <see cref="Peer"/> can send the given <see cref="OperationType"/>.</returns>
		public override bool CanSend(OperationType opType)
		{
			return opType == OperationType.Request && NetworkSendService.CanSend(opType);
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
			Throw<ArgumentNullException>.If.IsNull(payload)?.Now(nameof(payload));

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
			Throw<ArgumentNullException>.If.IsNull(payload)?.Now(nameof(payload));

			return NetworkSendService.TrySendMessage(OperationType.Request, payload);
		}

//Enduser clients shouldn't be routing messages.
//Example: A gameclient for a player. These sorts of clients do NOT need to route messages they recieve.
#if !ENDUSER
		/// <summary>
		/// Called internally when a response is recieved.
		/// </summary>
		/// <param name="payload"><see cref="IResponseMessage"/> sent by the peer.</param>
		/// <param name="parameters">Parameters the message was sent with.</param>
		private void OnInternalReceiveResponse(IResponseMessage responseMessage, IMessageParameters parameters)
		{
			//We should check if the message is routing back.
			//This is suggested in the GladNet2 routing specification
			//Under "Route-back Outside Userspace": https://github.com/HelloKitty/GladNet2.Specifications/blob/master/Routing/RoutingSpecification.md
			if (responseMessage.isRoutingBack)
			{
				//Right now we just pass on the parameters.
				messageRoutebackService.RouteResponse(responseMessage, parameters);
			}

			//GladNet2 routing specification dictates that we should push the AUID
			//into the routing stack:https://github.com/HelloKitty/GladNet2.Specifications/blob/master/Routing/RoutingSpecification.md
			responseMessage.Push(PeerDetails.ConnectionID);

			OnReceiveResponse(responseMessage, parameters);
		}
#endif

		/// <summary>
		/// Called internally when an event is recieved.
		/// </summary>
		/// <param name="eventMessage"><see cref="IEventMessage"/> sent by the peer.</param>
		/// <param name="parameters">Parameters the message was sent with.</param>
		protected abstract void OnReceiveEvent(IEventMessage eventMessage, IMessageParameters parameters);

		/// <summary>
		/// Called internally when a response is recieved.
		/// </summary>
		/// <param name="payload"><see cref="IResponseMessage"/> sent by the peer.</param>
		/// <param name="parameters">Parameters the message was sent with.</param>
		protected abstract void OnReceiveResponse(IResponseMessage responseMessage, IMessageParameters parameters);
	}
}
