using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public class NetworkMessageDataContainer<TNetworkMessageBuilder> : INetworkMessageFluentBuilder<TNetworkMessageBuilder>
		where TNetworkMessageBuilder : INetworkMessage
	{
		public INetworkMessageFactory Factory { get; set; }

		public PacketPayload Payload { get; set; }
	}
}
