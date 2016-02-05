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
	public abstract class ClientPeer : Peer, IClientNetworkMessageSender
	{
		public ClientPeer(ILogger logger, INetworkMessageSender sender, IConnectionDetails details, INetworkMessageSubscriptionService netMessageSubService)
			: base(logger, sender, details)
		{
			if (netMessageSubService == null)
				throw new ArgumentNullException(nameof(netMessageSubService), "Cannot create a peer with a null net message sub service. That would mean it cannot recieve messages.");

			//Subscribe to request messages
			netMessageSubService.SubscribeTo<RequestMessage>()
				.With(OnReceiveRequest);
		}

		public override bool CanSend(OperationType opType)
		{
			//Returns true if the opType matches ClientPeer opTypes AND the sender service can send the opType too.
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
		public SendResult SendResponse(PacketPayload payload, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
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
		public SendResult SendEvent(PacketPayload payload, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			return NetworkSendService.TrySendMessage(OperationType.Event, payload, deliveryMethod, encrypt, channel);
		}

		/// <summary>
		/// Sends a networked event.
		/// Additionally this message/payloadtype is known to have static send parameters and those will be used in transit.
		/// </summary>
		/// <typeparam name="TPacketType">Type of the packet payload.</typeparam>
		/// <param name="opType"><see cref="OperationType"/> of the message to send.</param>
		/// <param name="payload">Payload instance to be sent in the message that contains static message parameters.</param>
		/// <returns>Indication of the message send state.</returns>
		public SendResult SendEvent<TPacketType>(OperationType opType, TPacketType payload) 
			where TPacketType : PacketPayload, IStaticPayloadParameters
		{
			return NetworkSendService.TrySendMessage<TPacketType>(OperationType.Event, payload);
		}

		/// <summary>
		/// Sends a networked response.
		/// Additionally this message/payloadtype is known to have static send parameters and those will be used in transit.
		/// </summary>
		/// <typeparam name="TPacketType">Type of the packet payload.</typeparam>
		/// <param name="opType"><see cref="OperationType"/> of the message to send.</param>
		/// <param name="payload">Payload instance to be sent in the message that contains static message parameters.</param>
		/// <returns>Indication of the message send state.</returns>
		public SendResult SendResponse<TPacketType>(OperationType opType, TPacketType payload) 
			where TPacketType : PacketPayload, IStaticPayloadParameters
		{
			return NetworkSendService.TrySendMessage<TPacketType>(OperationType.Response, payload);
		}
		#endregion

		protected virtual void OnInvalidOperationRecieved(Type packetType, IMessageParameters parameters, PacketPayload payload)
		{
			//Don't do anything here. We'll let inheritors do something extra by overriding this
		}

		protected abstract void OnReceiveRequest(IRequestMessage message, IMessageParameters parameters);

		protected virtual void OnStatusChanged(NetStatus status)
		{
			//TODO: Logging if debug

			//TODO: Do internal handling for status change events that are ClientPeer specific.
		}
	}
}
