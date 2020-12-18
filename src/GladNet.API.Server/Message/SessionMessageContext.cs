using System;
using System.Collections.Generic;
using System.Text;

namespace GladNet
{
	//TODO: We need better naming. Kinda conflicts with the existing message context for handlers
	/// <summary>
	/// The context for a network message.
	/// </summary>
	/// <typeparam name="TPayloadWriteType"></typeparam>
	/// <typeparam name="TPayloadReadType"></typeparam>
	public sealed class SessionMessageContext<TPayloadWriteType, TPayloadReadType> 
		where TPayloadWriteType : class 
		where TPayloadReadType : class
	{
		/// <summary>
		/// The session associated with the message.
		/// </summary>
		public ManagedSession<TPayloadWriteType, TPayloadReadType> Session { get; }

		/// <summary>
		/// The network message.
		/// </summary>
		public NetworkIncomingMessage<TPayloadReadType> Message { get; }

		/// <inheritdoc />
		public SessionMessageContext(ManagedSession<TPayloadWriteType, TPayloadReadType> session, NetworkIncomingMessage<TPayloadReadType> message)
		{
			Session = session ?? throw new ArgumentNullException(nameof(session));
			Message = message ?? throw new ArgumentNullException(nameof(message));
		}
	}
}
