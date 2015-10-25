using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public sealed class LidgrenNetEngine : INetworkMessageSender
	{
		private readonly INetworkMessageFactoryProvider messageFactory;

		public LidgrenNetEngine(INetworkMessageFactoryProvider netMessageFactory)
		{
			messageFactory = netMessageFactory;
		}

		public SendResult TrySendMessage(OperationType opType, PacketPayload payload, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			//TODO: Handling of NetResults depending on certain connection states

			NetworkMessage message = null; //messageFactory.Create(opType, payload);

			if (message == null)
				throw new InvalidOperationException("Mesage factory failed to generate " + typeof(NetworkMessage) + " or generated an invalid message..");

			return SendMessage(message, deliveryMethod, encrypt, channel);
		}

		//Should just pass to the non-generic version.
		public SendResult TrySendMessage<TPacketType>(OperationType opType, TPacketType payload) where TPacketType : PacketPayload, IStaticPayloadParameters
		{
			return TrySendMessage(opType, payload, payload.DeliveryMethod, payload.Encrypted, payload.Channel);
		}

		private SendResult SendMessage(NetworkMessage message, DeliveryMethod deliveryMethod, bool encrypt, byte channel)
		{
			throw new NotImplementedException();
		}
	}
}
