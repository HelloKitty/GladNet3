using GladNet.Common;
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
		public ClientPeer(INetworkMessageSender sender, IConnectionDetails details)
			: base(sender, details)
		{

		}

		public override bool CanSend(NetworkMessage.OperationType opType)
		{
			return opType == NetworkMessage.OperationType.Event || opType == NetworkMessage.OperationType.Response;
		}

		#region Message Senders
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public NetworkMessage.SendResult SendResponse(PacketPayload payload, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			return TrySendMessage(NetworkMessage.OperationType.Response, payload, deliveryMethod, encrypt, channel);
		}

		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public NetworkMessage.SendResult SendEvent(PacketPayload payload, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			return TrySendMessage(NetworkMessage.OperationType.Event, payload, deliveryMethod, encrypt, channel);
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

		public override NetworkMessage.SendResult TrySendMessage(NetworkMessage.OperationType opType, PacketPayload payload, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
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
