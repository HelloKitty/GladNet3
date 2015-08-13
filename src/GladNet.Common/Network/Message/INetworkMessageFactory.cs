using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	public interface INetworkMessageFactory
	{
		NetworkMessage Create(NetworkMessage.OperationType opType, PacketPayload payload);
	}
}
