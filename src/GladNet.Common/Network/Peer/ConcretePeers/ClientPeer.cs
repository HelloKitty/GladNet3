using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	public abstract class ClientPeer : Peer, IClientMessageSender
	{
		public override bool CanSend(NetworkMessage.OperationType opType)
		{
			return opType == NetworkMessage.OperationType.Event || opType == NetworkMessage.OperationType.Response;
		}

		#region Message Senders
		public NetworkMessage.SendResult SendResponse<TResponsePacket>(TResponsePacket payload, NetworkMessage.DeliveryMethod deliveryMethod, byte responseCode, bool encrypt = false, byte channel = 0) where TResponsePacket : PacketPayload, IResponsePayload
		{
			throw new NotImplementedException();
		}

		public NetworkMessage.SendResult SendEvent<TEventPacket>(TEventPacket payload, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0) where TEventPacket : PacketPayload, IEventPayload
		{
			throw new NotImplementedException();
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

		}

		//We can override because we no this is invalid.
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public sealed override NetworkMessage.SendResult TrySendMessage(PacketPayload payload, IEventPayload eventParameters, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			//TODO: Implement sending.
			throw new NotImplementedException();
		}

		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public sealed override NetworkMessage.SendResult TrySendMessage(PacketPayload payload, IResponsePayload responseParameters, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			//TODO: Implement sending.
			throw new NotImplementedException();
		}

		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		//We can skip checks for efficiency and send directly.
		public sealed override NetworkMessage.SendResult TrySendMessage(PacketPayload payload, IRequestPayload requestParameters, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			//TODO: Logging.
			return NetworkMessage.SendResult.Invalid;
		}

		#region Message Receivers
		protected abstract override void OnReceiveRequest(IRequestMessage message, IMessageParameters parameters);
		#endregion
	}
}
