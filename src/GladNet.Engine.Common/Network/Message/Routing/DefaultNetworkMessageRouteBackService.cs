using Common.Logging;
using Easyception;
using GladNet.Common;
using GladNet.Message;
using GladNet.Payload;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace GladNet.Engine.Common
{
	/// <summary>
	/// Default implementation of the GladNet2 <see cref="INetworkMessageRouteBackService"/>.
	/// Refer to specification on GladNet2 routing for more information.
	/// </summary>
	public class DefaultNetworkMessageRouteBackService : INetworkMessageRouteBackService, IClassLogger
	{
		/// <summary>
		/// Services that can map routing IDs, which are AUIDs, to <see cref="INetPeer"/>s.
		/// </summary>
		private IAUIDService<INetPeer> peerAUIDMapCollection { get; }

		public ILog Logger { get; }

		public DefaultNetworkMessageRouteBackService(IAUIDService<INetPeer> peerCollection, ILog logger)
		{
			Throw<ArgumentNullException>.If.IsNull(peerCollection)?.Now(nameof(peerCollection));
			Throw<ArgumentNullException>.If.IsNull(logger)?.Now(nameof(logger));

			peerAUIDMapCollection = peerCollection;
			Logger = logger;
		}

		/// <summary>
		/// Routes a <see cref="IResponseMessage"/> using the provided <see cref="IMessageParameters"/>.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public SendResult RouteResponse(IResponseMessage message, IMessageParameters parameters)
		{
			return RouteMessage(message, parameters);
		}

		public SendResult RouteMessage<TMessageType>(TMessageType message, IMessageParameters parameters)
			where TMessageType : INetworkMessage, IRoutableMessage, IOperationTypeMappable
		{
			return RouteMessage(message, parameters.DeliveryMethod, parameters.Encrypted, parameters.Channel);
		}

		public SendResult RouteMessage<TMessageType>(TMessageType message, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
			where TMessageType : INetworkMessage, IRoutableMessage, IOperationTypeMappable
		{
			if (!message.isMessageRoutable)
			{
				Logger.Error($"Recieved {nameof(TMessageType)} that is unable to be routed.");
				return SendResult.Invalid;
			}

			//enable the message for internal routing
			message.isRoutingBack = true;

			//Pop the ID we should route to.
			int? AUID = message.Pop();

			if (!AUID.HasValue)
			{
				Logger.Error($"Recieved {nameof(TMessageType)} that is unable to be routed due to null AUID.");
				return SendResult.Invalid;
			}

			INetPeer peer = TryGetValidNetPeer(AUID.Value);

			//This is ok because it just means they disconnected before we could route back.
			if (peer == null)
			{
				return SendResult.Invalid;
			}
			else
				return peer.NetworkSendService.TryRouteMessage(message, deliveryMethod, encrypt, channel);
		}

		/// <summary>
		/// Routes a <see cref="IRequestMessage"/> using the provided <see cref="IMessageParameters"/>.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public SendResult RouteRequest(IRequestMessage message, IMessageParameters parameters)
		{
			return RouteMessage(message, parameters);
		}

		/// <summary>
		/// Routes a request payload using the provided <paramref name="routingDetails"/>
		/// provided.
		/// </summary>
		/// <param name="payload">Payload instance to be sent in the message.</param>
		/// <param name="deliveryMethod">The deseried <see cref="DeliveryMethod"/> of the message.</param>
		/// <param name="encrypt">Indicates if the message should be encrypted.</param>
		/// <param name="channel">Indicates the channel for this message to be sent over.</param>
		/// <returns>Indication of the message send state.</returns>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public SendResult RouteRequest(PacketPayload payload, IRoutableMessage routingDetails, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			if (!routingDetails.isMessageRoutable)
				Logger.Error($"Recieved {nameof(IRoutableMessage)} that is unable to be used for routing information.");

			//We need to actually create the request to be routed.
			RequestMessage message = new RequestMessage(payload);

			//We have to transfer routing information from the routing details to the request message we created
			routingDetails.ExportRoutingDataTo(message);

			return RouteMessage(message, deliveryMethod, encrypt, channel);
		}

		/// <summary>
		/// Routes a response payload using the provided <paramref name="routingDetails"/>
		/// provided.
		/// </summary>
		/// <param name="payload">Payload instance to be sent in the message.</param>
		/// <param name="deliveryMethod">The deseried <see cref="DeliveryMethod"/> of the message.</param>
		/// <param name="encrypt">Indicates if the message should be encrypted.</param>
		/// <param name="channel">Indicates the channel for this message to be sent over.</param>
		/// <returns>Indication of the message send state.</returns>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public SendResult RouteResponse(PacketPayload payload, IRoutableMessage routingDetails, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			if (!routingDetails.isMessageRoutable)
				Logger.Error($"Recieved {nameof(IRoutableMessage)} that is unable to be used for routing information.");

			//We need to actually create the request to be routed.
			ResponseMessage message = new ResponseMessage(payload);

			//We have to transfer routing information from the routing details to the request message we created
			routingDetails.ExportRoutingDataTo(message);

			return RouteMessage(message, deliveryMethod, encrypt, channel);
		}

		/// <summary>
		/// Routes a request payload using the provided <paramref name="routingDetails"/> provided
		/// Additionally this message/payloadtype is known to have static send parameters and those will be used in transit.
		/// </summary>
		/// <typeparam name="TPacketType">Type of the packet payload.</typeparam>
		/// <param name="payload">Payload instance to be sent in the message that contains static message parameters.</param>
		/// <returns>Indication of the message send state.</returns>
		public SendResult RouteRequest<TPacketType>(TPacketType payload, IRoutableMessage routingDetails)
			where TPacketType : PacketPayload, IStaticPayloadParameters
		{
			return RouteRequest(payload, routingDetails, payload.DeliveryMethod, payload.Encrypted, payload.Channel);
		}

		/// <summary>
		/// Routes a response payload using the provided <paramref name="routingDetails"/> provided
		/// Additionally this message/payloadtype is known to have static send parameters and those will be used in transit.
		/// </summary>
		/// <typeparam name="TPacketType">Type of the packet payload.</typeparam>
		/// <param name="payload">Payload instance to be sent in the message that contains static message parameters.</param>
		/// <returns>Indication of the message send state.</returns>
		public SendResult RouteResponse<TPacketType>(TPacketType payload, IRoutableMessage routingDetails)
			where TPacketType : PacketPayload, IStaticPayloadParameters
		{
			return RouteResponse(payload, routingDetails, payload.DeliveryMethod, payload.Encrypted, payload.Channel);
		}

		/// <summary>
		/// Attempts to retrive the <see cref="INetPeer"/> instances from the <see cref="IAUIDService{TAUIDMapType}"/>.
		/// </summary>
		/// <param name="AUID">AUID value.</param>
		/// <returns>The peer with the AUID or null if not found.</returns>
		private INetPeer TryGetValidNetPeer(int AUID)
		{
			//We must lock on the sync obj
			peerAUIDMapCollection.syncObj.EnterReadLock();
			try
			{
				//If the service has the AUID we should grab the peer
				if (peerAUIDMapCollection.ContainsKey(AUID))
				{
					return peerAUIDMapCollection[AUID];
				}
			}
			finally
			{
				peerAUIDMapCollection.syncObj.ExitReadLock();
			}

			//If it's not in the collection we'll reach this point
			return null;
		}
	}
}
