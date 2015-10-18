using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public sealed class LidgrenNetEngine : INetworkMessageSender
	{
		private readonly INetworkMessageFactory messageFactory;

		public LidgrenNetEngine(INetworkMessageFactory netMessageFactory)
		{
			messageFactory = netMessageFactory;
		}

		public NetworkMessage.SendResult TrySendMessage(NetworkMessage.OperationType opType, PacketPayload payload, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			//TODO: Handling of NetResults depending on certain connection states

			NetworkMessage message = messageFactory.Create(opType, payload);

			if (message == null)
				throw new InvalidOperationException("Mesage factory failed to generate " + typeof(NetworkMessage) + " or generated an invalid message..");

			return SendMessage(message, deliveryMethod, encrypt, channel);
		}

		private NetworkMessage.SendResult SendMessage(NetworkMessage message, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt, byte channel)
		{
			throw new NotImplementedException();
		}
	}
}
