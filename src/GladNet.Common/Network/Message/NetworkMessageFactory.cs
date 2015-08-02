using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	public class NetworkMessageFactory
	{
		public NetworkMessage Create(PacketPayload payload, IResponsePayload responseParameters)
		{
			return new ResponseMessage(payload, responseParameters.ResponseCode);
		}

		public NetworkMessage Create(PacketPayload payload, IRequestPayload responseParameters)
		{
			return new RequestMessage(payload);
		}

		public NetworkMessage Create(PacketPayload payload, IEventPayload responseParameters)
		{
			return new EventMessage(payload);
		}
	}
}
