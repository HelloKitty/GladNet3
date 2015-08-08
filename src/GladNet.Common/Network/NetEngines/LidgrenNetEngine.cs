using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	public sealed class LidgrenNetEngine : INetEngine
	{
		private readonly NetConnection lidgrenConnection;

		private readonly INetworkMessageFactory messageFactory;

		public IConnectionDetails Details
		{
			get { throw new NotImplementedException(); }
		}

		public LidgrenNetEngine(NetConnection netConnection, INetworkMessageFactory netMessageFactory)
		{
			lidgrenConnection = netConnection;
			messageFactory = netMessageFactory;
		}

		public NetworkMessage.SendResult TrySendMessage(PacketPayload payload, IRequestPayload requestParameters, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			NetworkMessage message = messageFactory.Create(payload, requestParameters);

			if (message == null)
				throw new NullReferenceException("Mesage factory failed to generate " + typeof(NetworkMessage) + " as it is null.");

			return SendMessage(message, deliveryMethod, encrypt, channel);
		}

		public NetworkMessage.SendResult TrySendMessage(PacketPayload payload, IEventPayload eventParameters, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			NetworkMessage message = messageFactory.Create(payload, eventParameters);

			if (message == null)
				throw new NullReferenceException("Mesage factory failed to generate " + typeof(NetworkMessage) + " as it is null.");

			return SendMessage(message, deliveryMethod, encrypt, channel);
		}

		public NetworkMessage.SendResult TrySendMessage(PacketPayload payload, IResponsePayload responseParameters, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			NetworkMessage message = messageFactory.Create(payload, responseParameters);

			if (message == null)
				throw new NullReferenceException("Mesage factory failed to generate " + typeof(NetworkMessage) + " as it is null.");

			return SendMessage(message, deliveryMethod, encrypt, channel);
		}

		public NetworkMessage.SendResult TrySendMessage(NetworkMessage.OperationType opType, PacketPayload payload, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			throw new NotImplementedException();
		}

		private NetworkMessage.SendResult SendMessage(NetworkMessage message, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt, byte channel)
		{
			throw new NotImplementedException();
		}
	}
}
