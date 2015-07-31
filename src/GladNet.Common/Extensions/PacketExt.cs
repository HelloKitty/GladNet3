using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common.Extensions
{
	public static class PacketExt
	{
		public static Packet.DeliveryMethod LidgrenDeliveryTypeToGladNetType(this NetDeliveryMethod lidgrenDeliveryMethod)
		{
			switch (lidgrenDeliveryMethod)
			{
				case NetDeliveryMethod.ReliableOrdered:
					return Packet.DeliveryMethod.ReliableOrdered;
				case NetDeliveryMethod.ReliableSequenced:
					return Packet.DeliveryMethod.ReliableDiscardStale;
				case NetDeliveryMethod.ReliableUnordered:
					return Packet.DeliveryMethod.ReliableUnordered;
				case NetDeliveryMethod.Unreliable:
					return Packet.DeliveryMethod.UnreliableAcceptDuplicate;
				case NetDeliveryMethod.UnreliableSequenced:
					return Packet.DeliveryMethod.UnreliableDiscardStale;

				default:
					throw new ArgumentException("LidgrenDeliveryMethod invalid in " + typeof(PacketExt) + " unable to translate to GladNet type. Invalid code was: "
						+ lidgrenDeliveryMethod.ToString(), "lidgrenDeliveryMethod");
			}
		}
	}
}
