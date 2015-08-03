using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

		public NetworkMessage.SendResult TrySendMessage(NetworkMessage.OperationType opType, PacketPayload payload, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			NetworkMessage message = messageFactory.Create(opType, payload);

			if (message == null)
				throw new NullReferenceException("Mesage factory failed to generate " + typeof(NetworkMessage) + " as it is null.");

			return SendMessage(message, deliveryMethod, encrypt, channel);
		}

		private NetworkMessage.SendResult SendMessage(NetworkMessage message, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt, byte channel)
		{
			throw new NotImplementedException();
		}
	}
}
