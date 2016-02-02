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
			return opType == OperationType.Response || opType == OperationType.Event;
		}

		#region Message Senders
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public SendResult SendResponse(PacketPayload payload, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			return TrySendMessage(OperationType.Response, payload, deliveryMethod, encrypt, channel);
		}

		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public SendResult SendEvent(PacketPayload payload, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			return TrySendMessage(OperationType.Event, payload, deliveryMethod, encrypt, channel);
		}

		public SendResult SendEvent<TPacketType>(OperationType opType, TPacketType payload) 
			where TPacketType : PacketPayload, IStaticPayloadParameters
		{
			return TrySendMessage<TPacketType>(OperationType.Event, payload);
		}

		public SendResult SendResponse<TPacketType>(OperationType opType, TPacketType payload) 
			where TPacketType : PacketPayload, IStaticPayloadParameters
		{
			return TrySendMessage<TPacketType>(OperationType.Response, payload);
		}
		#endregion

		protected virtual void OnInvalidOperationRecieved(Type packetType, IMessageParameters parameters, PacketPayload payload)
		{
			//Don't do anything here. We'll let inheritors do something extra by overriding this
		}

		public override SendResult TrySendMessage(OperationType opType, PacketPayload payload, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			return base.TrySendMessage(opType, payload, deliveryMethod, encrypt, channel);
		}

		#region Message Receivers
		protected abstract void OnReceiveRequest(IRequestMessage message, IMessageParameters parameters);
		#endregion

		protected override void OnStatusChanged(NetStatus status)
		{
			//TODO: Logging if debug

			//TODO: Do internal handling for status change events that are ClientPeer specific.
		}
	}
}
