using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	public abstract class Peer : INetPeer, INetworkMessageReceiver
	{
		#region Message Senders
		public virtual NetworkMessage.SendResult SendMessage(PacketPayload payload, PacketPayload.IRequest requestParameters, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt, byte channel)
		{
			throw new NotImplementedException();
		}

		public virtual NetworkMessage.SendResult SendMessage(PacketPayload payload, PacketPayload.IEvent eventParameters, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt, byte channel)
		{
			throw new NotImplementedException();
		}

		public virtual NetworkMessage.SendResult SendMessage(PacketPayload payload, PacketPayload.IResponse responseParameters, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt, byte channel)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Message Receivers
		void INetworkMessageReceiver.OnNetworkMessageReceive(IRequestMessage message, IMessageParameters parameters)
		{
			OnReceiveRequest(message, parameters);
		}

		void INetworkMessageReceiver.OnNetworkMessageReceive(IResponseMessage message, IMessageParameters parameters)
		{
			OnReceiveResponse(message, parameters);
		}

		void INetworkMessageReceiver.OnNetworkMessageReceive(IEventMessage message, IMessageParameters parameters)
		{
			OnReceiveEvent(message, parameters);
		}

		protected abstract void OnReceiveRequest(IRequestMessage message, IMessageParameters parameters);
		protected abstract void OnReceiveResponse(IResponseMessage message, IMessageParameters parameters);
		protected abstract void OnReceiveEvent(IEventMessage message, IMessageParameters parameters);
		#endregion
	}
}
