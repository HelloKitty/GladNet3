using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public class NetworkMessageFactory : INetworkMessageFactory
	{
		public EventMessage CreateEventMessage(PacketPayload payload)
		{
			payload.ThrowIfNull(nameof(payload));

			return new EventMessage(payload);
		}

		public RequestMessage CreateRequestMessage(PacketPayload payload)
		{
			payload.ThrowIfNull(nameof(payload));

			return new RequestMessage(payload);
		}

		public ResponseMessage CreateResponseMessage(PacketPayload payload)
		{
			payload.ThrowIfNull(nameof(payload));

			return new ResponseMessage(payload);
		}

		public StatusMessage CreateStatusMessage(StatusChangePayload payload)
		{
			payload.ThrowIfNull(nameof(payload));

			return new StatusMessage(payload);
		}
	}
}
