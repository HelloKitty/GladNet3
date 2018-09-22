using System;
using GladNet;
using JetBrains.Annotations;
using Moq;

namespace GladNet
{
	public sealed class GenericMessageContextFactory<TPayloadWriteType, TPayloadReadType> : IGenericMessageContextFactory<TPayloadWriteType, IProxiedMessageContext<TPayloadWriteType, TPayloadReadType>> 
		where TPayloadWriteType : class 
		where TPayloadReadType : class
	{
		private IManagedNetworkClient<TPayloadReadType, TPayloadWriteType> ProxyConnection { get; }

		private static IPeerRequestSendService<TPayloadWriteType> MockedPeerRequestService { get; }

		static GenericMessageContextFactory()
		{
			try
			{
				MockedPeerRequestService = Mock.Of<IPeerRequestSendService<TPayloadWriteType>>();
			}
			catch(Exception e)
			{
				throw new InvalidOperationException($"Failed to create Mocked peer request service for Type: {nameof(TPayloadWriteType)}", e);
				throw;
			}
		}

		public GenericMessageContextFactory([NotNull] IManagedNetworkClient<TPayloadReadType, TPayloadWriteType> proxyConnection)
		{
			ProxyConnection = proxyConnection ?? throw new ArgumentNullException(nameof(proxyConnection));
		}

		public IProxiedMessageContext<TPayloadWriteType, TPayloadReadType> CreateMessageContext(IConnectionService connectionService, IPeerPayloadSendService<TPayloadWriteType> sendService, SessionDetails details)
		{
			return new GenericProxiedMessageContext<TPayloadWriteType, TPayloadReadType>(ProxyConnection, connectionService, sendService, MockedPeerRequestService);
		}
	}
}