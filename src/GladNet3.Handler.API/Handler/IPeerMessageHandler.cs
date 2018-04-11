using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet
{
	/// <summary>
	/// Contract for types that provide message handling logic.
	/// Uses the default peer context type.
	/// </summary>
	/// <typeparam name="TIncomingPayloadType"></typeparam>
	/// <typeparam name="TOutgoingPayloadType"></typeparam>
	public interface IPeerMessageHandler<TIncomingPayloadType, out TOutgoingPayloadType> : IPeerMessageHandler<TIncomingPayloadType, TOutgoingPayloadType, IPeerMessageContext<TOutgoingPayloadType>>
		where TIncomingPayloadType : class
		where TOutgoingPayloadType : class
	{
		
	}

	/// <summary>
	/// Contract for types that provide message handling logic.
	/// </summary>
	/// <typeparam name="TIncomingPayloadType"></typeparam>
	/// <typeparam name="TOutgoingPayloadType"></typeparam>
	/// <typeparam name="TMessageContextType"></typeparam>
	public interface IPeerMessageHandler<TIncomingPayloadType, out TOutgoingPayloadType, in TMessageContextType>
		where TIncomingPayloadType : class
		where TOutgoingPayloadType : class
		where TMessageContextType : IPeerMessageContext<TOutgoingPayloadType>
	{
		/// <summary>
		/// Indicates if the handler can handle the provided <see cref="message"/>.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <returns>True if the handler can handle that message.</returns>
		bool CanHandle(NetworkIncomingMessage<TIncomingPayloadType> message);

		/// <summary>
		/// Tries to handle the provided <see cref="message"/>
		/// and indicates if the message has been consumed.
		/// </summary>
		/// <param name="message">The message to try to handle.</param>
		/// <param name="context">The context for the message.</param>
		/// <returns>
		/// True indicates that the message was handled and consumed. 
		/// False indicates that the handler couldn't handle the message.
		/// </returns>
		Task<bool> TryHandleMessage(TMessageContextType context, NetworkIncomingMessage<TIncomingPayloadType> message);
	}
}
