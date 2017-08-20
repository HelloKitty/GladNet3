using GladNet.Common;
using GladNet.Message;
using GladNet.Payload;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace GladNet.Engine.Common
{
	public static class INetPeerExtensions
	{
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public static SendResult TrySendMessage(this INetPeer peer, OperationType opType, PacketPayload payload, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			if (payload == null) throw new ArgumentNullException(nameof(payload));

			//TODO: Implement logging.
			if (!peer.CanSend(opType))
				return SendResult.Invalid;

			return peer.NetworkSendService.TrySendMessage(opType, payload, deliveryMethod, encrypt, channel); //ncrunch: no coverage Reason: The line doesn't have to be tested. This is abstract and can be overidden.
		}

		public static SendResult TrySendMessage<TPacketType>(this INetPeer peer, OperationType opType, TPacketType payload) where TPacketType 
			: PacketPayload, IStaticPayloadParameters
		{
			if (payload == null) throw new ArgumentNullException(nameof(payload));

			return TrySendMessage(peer, opType, payload, payload.DeliveryMethod, payload.Encrypted, payload.Channel);
		}
	}
}
