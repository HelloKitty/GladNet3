using System;
using GladNet;
using JetBrains.Annotations;

namespace GladNet
{
	public sealed class GenericProxiedMessageContext<TPayloadWriteType, TPayloadReadType> : IProxiedMessageContext<TPayloadWriteType, TPayloadReadType> 
		where TPayloadWriteType : class 
		where TPayloadReadType : class
	{
		public IManagedNetworkClient<TPayloadReadType, TPayloadWriteType> ProxyConnection { get; }

		public IConnectionService ConnectionService { get; }

		public IPeerPayloadSendService<TPayloadWriteType> PayloadSendService { get; }

		public IPeerRequestSendService<TPayloadWriteType> RequestSendService { get; }

		/// <inheritdoc />
		public GenericProxiedMessageContext([NotNull] IManagedNetworkClient<TPayloadReadType, TPayloadWriteType> proxyConnection, [NotNull] IConnectionService connectionService, [NotNull] IPeerPayloadSendService<TPayloadWriteType> payloadSendService, [NotNull] IPeerRequestSendService<TPayloadWriteType> requestSendService)
		{
			ProxyConnection = proxyConnection ?? throw new ArgumentNullException(nameof(proxyConnection));
			ConnectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
			PayloadSendService = payloadSendService ?? throw new ArgumentNullException(nameof(payloadSendService));
			RequestSendService = requestSendService ?? throw new ArgumentNullException(nameof(requestSendService));
		}
	}
}