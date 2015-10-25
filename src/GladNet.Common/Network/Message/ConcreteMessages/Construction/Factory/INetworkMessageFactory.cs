using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public interface INetworkMessageFactory
	{
		NetworkMessage With<TPayload>(TPayload payload)
			where TPayload : PacketPayload;

		NetworkMessage WithDefaultConstructed<TPayload>()
			where TPayload : PacketPayload, new();
	}
}
