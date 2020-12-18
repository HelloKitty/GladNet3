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
		public ManagedSession Session { get; }

		/// <summary>
		/// The network message.
		/// </summary>
		public NetworkIncomingMessage<TPayloadReadType> Message { get; }

		/// <summary>
		/// The network message send service.
		/// </summary>
		public IMessageSendService<TPayloadWriteType> SendService { get; }

		/// <inheritdoc />
		public SessionMessageContext(ManagedSession session, NetworkIncomingMessage<TPayloadReadType> message, IMessageSendService<TPayloadWriteType> sendService)
		{
			Session = session ?? throw new ArgumentNullException(nameof(session));
			Message = message ?? throw new ArgumentNullException(nameof(message));
			SendService = sendService ?? throw new ArgumentNullException(nameof(sendService));
		}
	}
}
