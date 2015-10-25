using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public interface INetworkMessageFactoryProviderRegister
	{
		INetworkMessageFactoryProviderRegister Register<TNetworkMessage>(INetworkMessageFactory factory)
			where TNetworkMessage : NetworkMessage;

		INetworkMessageFactoryProviderRegister Register(OperationType opType, INetworkMessageFactory factory);
	}
}
