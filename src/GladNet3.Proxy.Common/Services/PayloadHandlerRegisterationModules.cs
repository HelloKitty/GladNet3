using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace GladNet
{
	public class PayloadHandlerRegisterationModules<TPayloadReadType, TPayloadWriteType> 
		where TPayloadReadType : class 
		where TPayloadWriteType : class
	{
		/// <summary>
		/// Implementers should return a collection of payload handler modules
		/// that they would like to proxy, acting as the server, to use.
		/// Do NOT register client modules like this. Client modules with their own handlers are seperate.
		/// </summary>
		/// <returns></returns>
		public IReadOnlyCollection<PayloadHandlerRegisterationModule<TPayloadReadType, TPayloadWriteType, IProxiedMessageContext<TPayloadWriteType, TPayloadReadType>>> ClientMessageHandlerModules { get; }

		/// <summary>
		/// Implementers should return a collection of payload handler modules
		/// that they would like to roxy, acting as the client, to use.
		/// Do NOT register server modules like this. Server modules with their own handlers are seperate.
		/// </summary>
		/// <returns></returns>
		public IReadOnlyCollection<PayloadHandlerRegisterationModule<TPayloadWriteType, TPayloadReadType, IProxiedMessageContext<TPayloadReadType, TPayloadWriteType>>> ServerMessageHandlerModules { get; }

		/// <inheritdoc />
		public PayloadHandlerRegisterationModules([NotNull] IReadOnlyCollection<PayloadHandlerRegisterationModule<TPayloadReadType, TPayloadWriteType, IProxiedMessageContext<TPayloadWriteType, TPayloadReadType>>> clientMessageHandlerModules, [NotNull] IReadOnlyCollection<PayloadHandlerRegisterationModule<TPayloadWriteType, TPayloadReadType, IProxiedMessageContext<TPayloadReadType, TPayloadWriteType>>> serverMessageHandlerModules)
		{
			ServerMessageHandlerModules = serverMessageHandlerModules ?? throw new ArgumentNullException(nameof(serverMessageHandlerModules));
			ClientMessageHandlerModules = clientMessageHandlerModules ?? throw new ArgumentNullException(nameof(clientMessageHandlerModules));
		}
	}
}
