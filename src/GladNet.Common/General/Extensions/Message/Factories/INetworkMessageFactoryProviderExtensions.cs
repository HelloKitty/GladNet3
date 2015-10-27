using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public static class INetworkMessageFactoryProviderExtensions
	{
		public static INetworkMessageFactory<TNetworkMessageType> CreateType<TNetworkMessageType>(this INetworkMessageFactoryProvider provider)
			where TNetworkMessageType : NetworkMessage
		{
			return provider.GetFactoryFor<TNetworkMessageType>() as INetworkMessageFactory<TNetworkMessageType>;
		}

		public static INetworkMessageFactory CreateType(this INetworkMessageFactoryProvider provider, OperationType type)
		{
			return provider.GetFactoryFor(type);
		}
	}
}
