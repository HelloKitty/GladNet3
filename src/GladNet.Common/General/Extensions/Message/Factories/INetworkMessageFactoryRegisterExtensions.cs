using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public static class INetworkMessageFactoryRegisterExtensions
	{
		public static TNetworkFacRegType RegisterAs<TNetworkMessageType, TNetworkFacRegType>(this TNetworkFacRegType register, INetworkMessageFactory<TNetworkMessageType> factory)
			where TNetworkMessageType : NetworkMessage
			where TNetworkFacRegType : class, INetworkMessageFactoryProviderRegister
		{
			register.Register<TNetworkMessageType>(factory);
			return register;
		}

		public static TNetworkFacRegType RegisterAs<TNetworkMessageType, TNetworkFacRegType>(this TNetworkFacRegType register, Func<PacketPayload, TNetworkMessageType> messageFunc)
			where TNetworkMessageType : NetworkMessage
			where TNetworkFacRegType : class, INetworkMessageFactoryProviderRegister
		{
			register.Register<TNetworkMessageType>(new GeneralNetworkMessageFactory<TNetworkMessageType>(messageFunc));
			return register;
		}
	}
}
