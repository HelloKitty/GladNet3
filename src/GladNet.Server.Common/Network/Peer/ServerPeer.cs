using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logging.Services;
using System.Diagnostics.CodeAnalysis;

namespace GladNet.Server.Common
{
	public abstract class ServerPeer : Peer, IClientPeerNetworkMessageSender
	{
		protected ServerPeer(ILogger logger, INetworkMessageSender messageSender, IConnectionDetails details, INetworkMessageSubscriptionService subService)
			: base(logger, messageSender, details, subService)
		{
			subService.ThrowIfNull(nameof(subService));

			//ServerPeers should be interested in events and responses from the server they are a peer of
			subService.SubscribeTo<EventMessage>()
				.With((x, y) => OnReceiveEvent(x.Payload.Data, y));

			subService.SubscribeTo<ResponseMessage>()
				.With((x, y) => OnReceiveResponse(x.Payload.Data, y));
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
			payload.ThrowIfNull(nameof(payload));

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
			payload.ThrowIfNull(nameof(payload));

			return NetworkSendService.TrySendMessage(OperationType.Request, payload);
		}

		/// <summary>
		/// Called internally when an event is recieved.
		/// </summary>
		/// <param name="payload"><see cref="PacketPayload"/> sent by the peer.</param>
		/// <param name="parameters">Parameters the message was sent with.</param>
		protected abstract void OnReceiveEvent(PacketPayload payload, IMessageParameters parameters);

		/// <summary>
		/// Called internally when a response is recieved.
		/// </summary>
		/// <param name="payload"><see cref="PacketPayload"/> sent by the peer.</param>
		/// <param name="parameters">Parameters the message was sent with.</param>
		protected abstract void OnReceiveResponse(PacketPayload payload, IMessageParameters parameters);
	}
}
