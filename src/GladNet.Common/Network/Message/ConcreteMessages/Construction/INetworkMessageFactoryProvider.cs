using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GladNet.Common
{
	public interface INetworkMessageFactoryProvider
	{
		INetworkMessageFactory GetFactoryFor(OperationType opType);

		INetworkMessageFactory GetFactoryFor<TNetworkMessage>()
			where TNetworkMessage : NetworkMessage;
	}
}
