using GladNet.Message;
using GladNet.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Engine.Common
{
	public class NetworkMessageFactory : INetworkMessageFactory
	{
		public EventMessage CreateEventMessage(PacketPayload payload)
		{
			if (payload == null) throw new ArgumentNullException(nameof(payload));

			return new EventMessage(payload);
		}

		public RequestMessage CreateRequestMessage(PacketPayload payload)
		{
			if (payload == null) throw new ArgumentNullException(nameof(payload));

			return new RequestMessage(payload);
		}

		public ResponseMessage CreateResponseMessage(PacketPayload payload)
		{
			if (payload == null) throw new ArgumentNullException(nameof(payload));

			return new ResponseMessage(payload);
		}

		public StatusMessage CreateStatusMessage(StatusChangePayload payload)
		{
			if (payload == null) throw new ArgumentNullException(nameof(payload));

			return new StatusMessage(payload);
		}
	}
}
