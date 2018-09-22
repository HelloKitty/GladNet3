using System;
using System.Collections.Generic;
using System.Text;
using GladNet;
using JetBrains.Annotations;

namespace GladNet
{
	/// <summary>
	/// The default/generic proxied managed session that uses a <see cref="IProxiedMessageContext{TPayloadWriteType,TPayloadReadType}"/>
	/// as the message context.
	/// </summary>
	/// <typeparam name="TPayloadWriteType"></typeparam>
	/// <typeparam name="TPayloadReadType"></typeparam>
	public sealed class GenericProxiedManagedClientSession<TPayloadWriteType, TPayloadReadType> : ProxiedManagedClientSession<TPayloadWriteType, TPayloadReadType, IProxiedMessageContext<TPayloadWriteType, TPayloadReadType>> 
		where TPayloadReadType : class 
		where TPayloadWriteType : class
	{
		/// <inheritdoc />
		public GenericProxiedManagedClientSession(IManagedNetworkServerClient<TPayloadWriteType, TPayloadReadType> internalManagedNetworkClient, SessionDetails details, [NotNull] MessageHandlerService<TPayloadReadType, TPayloadWriteType, IProxiedMessageContext<TPayloadWriteType, TPayloadReadType>> authMessageHandlerService, IGenericMessageContextFactory<TPayloadWriteType, IProxiedMessageContext<TPayloadWriteType, TPayloadReadType>> messageContextFactory) 
			: base(internalManagedNetworkClient, details, authMessageHandlerService, messageContextFactory)
		{

		}
	}
}
