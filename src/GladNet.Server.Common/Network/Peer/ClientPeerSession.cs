using GladNet.Common;
using Logging.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Server.Common
{
	public abstract class ClientPeerSession : Peer, IClientSessionNetworkMessageSender
	{
		public ClientPeerSession(ILogger logger, INetworkMessageSender sender, IConnectionDetails details, INetworkMessageSubscriptionService subService,
			IDisconnectionServiceHandler disconnectHandler)
				: base(logger, sender, details, subService, disconnectHandler)
		{
			subService.ThrowIfNull(nameof(subService));

			//Subscribe to request messages
			subService.SubscribeTo<RequestMessage>()
				.With((x, y) => OnReceiveRequest(x.Payload.Data, y));
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
			payload.ThrowIfNull(nameof(payload));

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
			payload.ThrowIfNull(nameof(payload));

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
			payload.ThrowIfNull(nameof(payload));

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
			payload.ThrowIfNull(nameof(payload));

			return NetworkSendService.TrySendMessage<TPacketType>(OperationType.Response, payload);
		}
		#endregion

		/// <summary>
		/// Called internally when a request is recieved from the remote peer.
		/// </summary>
		/// <param name="payload"><see cref="PacketPayload"/> sent by the peer.</param>
		/// <param name="parameters">Parameters the message was sent with.</param>
		protected abstract void OnReceiveRequest(PacketPayload payload, IMessageParameters parameters);
	}
}
