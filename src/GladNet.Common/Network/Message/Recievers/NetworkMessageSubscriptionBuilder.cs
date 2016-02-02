using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public class NetworkMessageSubscriptionBuilder<TNetworkMessageType> : INetworkMessageSubcriptionBuilder<TNetworkMessageType>
		where TNetworkMessageType : INetworkMessage
	{
		public INetworkMessageSubscriptionService Service { get; private set; }

		public NetworkMessageSubscriptionBuilder(INetworkMessageSubscriptionService service)
		{
			Service = service;
		}
	}
}
