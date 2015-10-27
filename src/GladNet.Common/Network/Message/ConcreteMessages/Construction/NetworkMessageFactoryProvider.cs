using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GladNet.Common
{
	public class NetworkMessageFactoryProvider : INetworkMessageFactoryProvider, INetworkMessageFactoryProviderRegister
	{
		private readonly Dictionary<Type, INetworkMessageFactory> factoryMap;

		public static NetworkMessageFactoryProvider Create()
		{
			return new NetworkMessageFactoryProvider();
		}

		public NetworkMessageFactoryProvider()
		{
			factoryMap = new Dictionary<Type, INetworkMessageFactory>(3);
		}

		public INetworkMessageFactory GetFactoryFor(OperationType opType)
		{
			return CreateType(opType.ToNetworkMessageType());
		}

		public INetworkMessageFactory GetFactoryFor<TNetworkMessage>() 
			where TNetworkMessage : NetworkMessage
		{
			return CreateType(typeof(TNetworkMessage));
		}

		public void Register<TNetworkMessage>(INetworkMessageFactory factory)
			where TNetworkMessage : NetworkMessage
		{
			Register(typeof(TNetworkMessage), factory);
		}

		private void Register(Type messageType, INetworkMessageFactory factory)
		{
			if (factory == null)
				throw new ArgumentNullException("factory", "Parameter is null. Expects a non-null factory for registeration.");

			factoryMap[messageType] = factory;
		}

		public void Register(OperationType opType, INetworkMessageFactory factory)
		{
			Register(opType.ToNetworkMessageType(), factory);
		}

		private INetworkMessageFactory CreateType(Type networkType)
		{
#if DEBUG || DEBUG_BUILD
			if(!networkType.IsSubclassOf(typeof(NetworkMessage)))
				throw new ArgumentException("Parameter was not a subclass of NetworkMessage.", "networkType");
#endif

			if (!factoryMap.ContainsKey(networkType))
				throw new InvalidOperationException(typeof(NetworkMessageFactoryProvider) + " does not have a registered factory for Type: " + networkType + ".");

			return factoryMap[networkType];
		}
	}
}
