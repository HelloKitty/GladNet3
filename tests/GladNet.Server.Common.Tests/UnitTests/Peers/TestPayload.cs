using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Server.Common.Tests
{
	public class TestPayload : PacketPayload, IStaticPayloadParameters
	{
		public byte Channel { get { return 5; } }

		public DeliveryMethod DeliveryMethod { get { return GladNet.Common.DeliveryMethod.ReliableOrdered; } }

		public bool Encrypted { get { return true; } }
	}
}
