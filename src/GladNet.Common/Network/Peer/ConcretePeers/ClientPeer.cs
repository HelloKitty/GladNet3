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
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public NetworkMessage.SendResult SendRequest<TRequestPacketType>(TRequestPacketType payload, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
			where TRequestPacketType : PacketPayload, IRequestPayload
		{
			//return ((IClientMessageSender)(this)).SendMessage(NetworkMessage.OperationType.Request, payload, deliveryMethod, encrypt, channel);
			throw new NotImplementedException();
		}

		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public NetworkMessage.SendResult SendRequest(PacketPayload payload, IRequestPayload requestParameters, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			//return ((IClientMessageSender)(this)).SendMessage(NetworkMessage.OperationType.Request, payload, deliveryMethod, encrypt, channel);
			throw new NotImplementedException();
		}

		protected override void OnReceiveRequest(IRequestMessage message, IMessageParameters parameters)
		{
			//ClientPeers don't handle requests. They send them.
			//If this is occuring in live production it is the result of likely packet forging.

			//TODO: Logging.
		}

		#region Message Receivers
		protected override abstract void OnReceiveResponse(IResponseMessage message, IMessageParameters parameters);

		protected override abstract void OnReceiveEvent(IEventMessage message, IMessageParameters parameters);
		#endregion

		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public NetworkMessage.SendResult SendMessage(NetworkMessage.OperationType opType, PacketPayload payload, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			throw new NotImplementedException();
		}
	}
}
