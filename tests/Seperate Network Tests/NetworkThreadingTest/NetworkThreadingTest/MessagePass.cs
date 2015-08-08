using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkThreadingTest
{
	public class MessagePass
	{
		public IEncryptor encryptor;

		public NetworkMessage.DeliveryMethod deliverMethod;

		public bool shouldEncrypt;

		public byte channel;

		public PacketPayload payload;
	}
}
