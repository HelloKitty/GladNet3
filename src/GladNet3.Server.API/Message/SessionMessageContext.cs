using System;
using System.Collections.Generic;
using System.Text;

namespace GladNet
{
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
		public ManagedClientSession<TPayloadWriteType, TPayloadReadType> Session { get; }

		/// <summary>
		/// The network message.
		/// </summary>
		public NetworkIncomingMessage<TPayloadReadType> Message { get; }

		/// <inheritdoc />
		public SessionMessageContext(ManagedClientSession<TPayloadWriteType, TPayloadReadType> session, NetworkIncomingMessage<TPayloadReadType> message)
		{
			if(session == null) throw new ArgumentNullException(nameof(session));
			if(message == null) throw new ArgumentNullException(nameof(message));

			Session = session;
			Message = message;
		}
	}
}
