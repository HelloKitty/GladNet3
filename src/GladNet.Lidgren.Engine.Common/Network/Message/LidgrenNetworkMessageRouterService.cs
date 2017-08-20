using GladNet.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GladNet.Common;
using GladNet.Message;
using GladNet.Payload;
using Lidgren.Network;
using GladNet.Lidgren.Common;

namespace GladNet.Lidgren.Engine.Common
{
	/// <summary>
	/// Abstract contract for message routing/sending service for Lidgren peers/connections.
	/// </summary>
	public abstract class LidgrenNetworkMessageRouterService : INetworkMessagePayloadSenderService
	{
		private INetworkMessageFactory networkMessageFactory { get; }

		protected NetConnection lidgrenNetworkConnection { get; } //protected so child can handle sending

		protected LidgrenNetworkMessageRouterService(INetworkMessageFactory messageFactory, NetConnection connection)
		{
			if (messageFactory == null)
				throw new ArgumentNullException(nameof(messageFactory), $"Cannot provide a null {nameof(INetworkMessageFactory)} service.");

			if (connection == null)
				throw new ArgumentNullException(nameof(connection), $"Cannot provide a null {nameof(NetConnection)} service.");

			lidgrenNetworkConnection = connection;
			networkMessageFactory = messageFactory;
		}

		protected abstract NetSendResult SendMessage(INetworkMessage message, DeliveryMethod deliveryMethod, bool encrypt, byte channel);

		public abstract bool CanSend(OperationType opType);

		public SendResult TrySendMessage(OperationType opType, PacketPayload payload, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			if (payload == null) throw new ArgumentNullException(nameof(payload));

			if (!CanSend(opType))
				throw new InvalidOperationException($"Cannot send {opType} with the {this.GetType().Name} because the service cannot handle that {nameof(OperationType)}.");

			INetworkMessage message = networkMessageFactory.Create(opType, payload);

			return SendValidMessage(message, deliveryMethod, encrypt, channel);
		}

		private SendResult SendValidMessage(INetworkMessage message, DeliveryMethod deliveryMethod, bool encrypt, byte channel)
		{
			return SendMessage(message, deliveryMethod, encrypt, channel).ToGladNet();
		}

		//TODO: This should be an extension method in GladNet.
		public SendResult TrySendMessage<TPacketType>(OperationType opType, TPacketType payload) 
			where TPacketType : PacketPayload, IStaticPayloadParameters
		{
			if (payload == null) throw new ArgumentNullException(nameof(payload));

			return TrySendMessage(opType, payload, payload.DeliveryMethod, payload.Encrypted, payload.Channel);
		}
	}
}
