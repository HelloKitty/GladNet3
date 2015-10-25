using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GladNet.Common
{
	public interface INetworkMessageFactoryProvider
	{
		INetworkMessageFactory CreateType(OperationType opType);

		INetworkMessageFactory CreateType<TNetworkMessage>()
			where TNetworkMessage : NetworkMessage;
	}
}
