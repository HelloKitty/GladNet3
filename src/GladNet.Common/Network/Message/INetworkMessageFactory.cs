using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	public interface INetworkMessageFactory
	{
		NetworkMessage Create(PacketPayload payload, IResponsePayload responseParameters);

		NetworkMessage Create(PacketPayload payload, IRequestPayload requestParameters);

		NetworkMessage Create(PacketPayload payload, IEventPayload eventParameters);

		NetworkMessage Create(NetworkMessage.OperationType opType, PacketPayload payload);
	}
}
