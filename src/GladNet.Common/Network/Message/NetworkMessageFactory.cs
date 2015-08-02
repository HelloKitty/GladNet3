using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	public class NetworkMessageFactory
	{
		public NetworkMessage Create(PacketPayload payload, PacketPayload.IResponse responseParameters)
		{
			return new ResponseMessage(payload, responseParameters.ResponseCode);
		}

		public NetworkMessage Create(PacketPayload payload, PacketPayload.IRequest responseParameters)
		{
			return new RequestMessage(payload);
		}

		public NetworkMessage Create(PacketPayload payload, PacketPayload.IEvent responseParameters)
		{
			return new EventMessage(payload);
		}
	}
}
