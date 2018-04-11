using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet
{
	/// <summary>
	/// Extension methods for handler types.
	/// </summary>
	public static class GladNetHandlerExtensions
	{
		/// <summary>
		/// Deocrates the payload handler in a generalized message handler with try semantics.
		/// This handler will try to consume messages and if not will indicate that the message wasn't consumed.
		/// This allows callers to try to handle a message that they don't know the payload type of and look for another consumer
		/// if it fails.
		/// </summary>
		/// <typeparam name="TPayloadType">The payload type (likely inferred)</typeparam>
		/// <typeparam name="TIncomingPayloadType"></typeparam>
		/// <typeparam name="TOutgoingPayloadType"></typeparam>
		/// <param name="handler">The non-null handler to decorate.</param>
		/// <returns>A new generalized message handler with try semantics.</returns>
		public static IPeerMessageHandler<TIncomingPayloadType, TOutgoingPayloadType> AsTryHandler<TPayloadType, TIncomingPayloadType, TOutgoingPayloadType>(this IPeerPayloadSpecificMessageHandler<TPayloadType, TOutgoingPayloadType, IPeerMessageContext<TOutgoingPayloadType>> handler)
			where TPayloadType : class, TIncomingPayloadType
			where TOutgoingPayloadType : class
			where TIncomingPayloadType : class
		{
			if(handler == null) throw new ArgumentNullException(nameof(handler));

			//We decorate the handler in try semantics
			return new TrySemanticsBasedOnTypePeerMessageHandler<TIncomingPayloadType, TOutgoingPayloadType, TPayloadType>(handler);
		}

		/// <summary>
		/// Deocrates the payload handler in a generalized message handler with try semantics.
		/// This handler will try to consume messages and if not will indicate that the message wasn't consumed.
		/// This allows callers to try to handle a message that they don't know the payload type of and look for another consumer
		/// if it fails.
		/// </summary>
		/// <typeparam name="TPayloadType">The payload type (likely inferred)</typeparam>
		/// <typeparam name="TIncomingPayloadType"></typeparam>
		/// <typeparam name="TOutgoingPayloadType"></typeparam>
		/// <param name="handler">The non-null handler to decorate.</param>
		/// <returns>A new generalized message handler with try semantics.</returns>
		public static IPeerMessageHandler<TIncomingPayloadType, TOutgoingPayloadType, TContextType> AsTryHandler<TPayloadType, TIncomingPayloadType, TOutgoingPayloadType, TContextType>(this IPeerPayloadSpecificMessageHandler<TPayloadType, TOutgoingPayloadType, TContextType> handler)
			where TPayloadType : class, TIncomingPayloadType
			where TOutgoingPayloadType : class
			where TIncomingPayloadType : class
			where TContextType : IPeerMessageContext<TOutgoingPayloadType>
		{
			if(handler == null) throw new ArgumentNullException(nameof(handler));

			//We decorate the handler in try semantics
			return new TrySemanticsBasedOnTypePeerMessageHandler<TIncomingPayloadType, TOutgoingPayloadType, TPayloadType, TContextType>(handler);
		}
	}
}
