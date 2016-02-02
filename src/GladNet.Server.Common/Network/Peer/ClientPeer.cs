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
		public ClientPeer(ILogger logger, INetworkMessageSender sender, IConnectionDetails details)
			: base(logger, sender, details)
		{

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

		protected sealed override void OnReceiveResponse(IResponseMessage message, IMessageParameters parameters)
		{
			//ClientPeers don't handle events. They send them.
			//If this is occuring in live production it is the result of likely packet forging.

			//TODO: Logging.
			//We call a virtual to let users do additional things if they'd like to override
			OnInvalidOperationRecieved(message.GetType(), parameters, message as PacketPayload);
		}

		protected sealed override void OnReceiveEvent(IEventMessage message, IMessageParameters parameters)
		{
			//ClientPeers don't handle events. They send them.
			//If this is occuring in live production it is the result of likely packet forging.

			//TODO: Logging.
			//We call a virtual to let users do additional things if they'd like to override
			OnInvalidOperationRecieved(message.GetType(), parameters, message as PacketPayload);
		}

		protected virtual void OnInvalidOperationRecieved(Type packetType, IMessageParameters parameters, PacketPayload payload)
		{
			//Don't do anything here. We'll let inheritors do something extra by overriding this
		}

		public override SendResult TrySendMessage(OperationType opType, PacketPayload payload, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			return base.TrySendMessage(opType, payload, deliveryMethod, encrypt, channel);
		}

		#region Message Receivers
		protected abstract override void OnReceiveRequest(IRequestMessage message, IMessageParameters parameters);
		#endregion

		protected override void OnStatusChanged(NetStatus status)
		{
			//TODO: Logging if debug

			//TODO: Do internal handling for status change events that are ClientPeer specific.
		}
	}
}
