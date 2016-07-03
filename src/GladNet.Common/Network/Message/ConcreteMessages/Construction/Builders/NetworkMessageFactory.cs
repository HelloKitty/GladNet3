using Easyception;
using GladNet.Payload;
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
			Throw<ArgumentNullException>.If.IsNull(payload)
				?.Now(nameof(Payload));

			return new EventMessage(payload);
		}

		public RequestMessage CreateRequestMessage(PacketPayload payload)
		{
			Throw<ArgumentNullException>.If.IsNull(payload)
				?.Now(nameof(Payload));

			return new RequestMessage(payload);
		}

		public ResponseMessage CreateResponseMessage(PacketPayload payload)
		{
			Throw<ArgumentNullException>.If.IsNull(payload)
				?.Now(nameof(Payload));

			return new ResponseMessage(payload);
		}

		public StatusMessage CreateStatusMessage(StatusChangePayload payload)
		{
			Throw<ArgumentNullException>.If.IsNull(payload)
				?.Now(nameof(Payload));

			return new StatusMessage(payload);
		}
	}
}
