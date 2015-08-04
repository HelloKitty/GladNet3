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
			return opType == NetworkMessage.OperationType.Request;
		}

		#region Message Senders
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public NetworkMessage.SendResult SendRequest<TRequestPacket>(TRequestPacket payload, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
			where TRequestPacket : PacketPayload, IRequestPayload
		{
			throw new NotImplementedException();
		}

		/*[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public NetworkMessage.SendResult SendRequest(PacketPayload payload, IRequestPayload requestParameters, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			throw new NotImplementedException();
		}*/

		protected override void OnReceiveRequest(IRequestMessage message, IMessageParameters parameters)
		{
			//ClientPeers don't handle requests. They send them.
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
		public override NetworkMessage.SendResult TrySendMessage(PacketPayload payload, IEventPayload eventParameters, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			//TODO: Logging.
			return NetworkMessage.SendResult.Invalid;
		}

		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public override NetworkMessage.SendResult TrySendMessage(PacketPayload payload, IResponsePayload responseParameters, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			//TODO: Logging.
			return NetworkMessage.SendResult.Invalid;
		}

		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		//We can skip checks for efficiency and send directly.
		public override NetworkMessage.SendResult TrySendMessage(PacketPayload payload, IRequestPayload requestParameters, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			//TODO: Implement sending.
			throw new NotImplementedException();
		}
		#endregion


		#region Message Receivers
		protected override abstract void OnReceiveResponse(IResponseMessage message, IMessageParameters parameters);

		protected override abstract void OnReceiveEvent(IEventMessage message, IMessageParameters parameters);
		#endregion
	}
}
