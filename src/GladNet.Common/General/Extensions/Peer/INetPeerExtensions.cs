using Easyception;
using GladNet.Payload;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public static class INetPeerExtensions
	{
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public static SendResult TrySendMessage(this INetPeer peer, OperationType opType, PacketPayload payload, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			Throw<ArgumentNullException>.If.IsNull(payload)
				?.Now(nameof(Payload));

			//TODO: Implement logging.
			if (!peer.CanSend(opType))
				return SendResult.Invalid;

			return peer.NetworkSendService.TrySendMessage(opType, payload, deliveryMethod, encrypt, channel); //ncrunch: no coverage Reason: The line doesn't have to be tested. This is abstract and can be overidden.
		}

		public static SendResult TrySendMessage<TPacketType>(this INetPeer peer, OperationType opType, TPacketType payload) where TPacketType : PacketPayload, IStaticPayloadParameters
		{
			Throw<ArgumentNullException>.If.IsNull(payload)
				?.Now(nameof(Payload));

			return TrySendMessage(peer, opType, payload, payload.DeliveryMethod, payload.Encrypted, payload.Channel);
		}
	}
}
