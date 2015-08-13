using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	public class NetworkMessageFactory : INetworkMessageFactory
	{

		public NetworkMessage Create(NetworkMessage.OperationType opType, PacketPayload payload)
		{
			if (payload == null)
				throw new ArgumentNullException("payload", "Payload is null in Response NetworkMessage factory method.");

			throw new NotImplementedException();
		}
	}
}
