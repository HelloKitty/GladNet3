using GladNet.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public interface INetworkMessageFluentBuilder<TNetworkMessageType>
		where TNetworkMessageType : INetworkMessage
	{
		PacketPayload Payload { get; set; }

		INetworkMessageFactory Factory { get; set; }
	}
}
