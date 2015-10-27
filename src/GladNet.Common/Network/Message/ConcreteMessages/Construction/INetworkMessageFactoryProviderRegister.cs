using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public interface INetworkMessageFactoryProviderRegister
	{
		void Register<TNetworkMessage>(INetworkMessageFactory factory)
			where TNetworkMessage : NetworkMessage;

		void Register(OperationType opType, INetworkMessageFactory factory);
	}
}
