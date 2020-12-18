using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet
{
	/// <summary>
	/// Contract for handlers that handle a specific derived type payload <typeparamref name="TMessageType"/>
	/// that derives from <see cref="TBaseMessageType"/>.
	/// </summary>
	/// <typeparam name="TMessageType">The type of message to be handled.</typeparam>
	/// <typeparam name="TMessageContext">The context associated with the message.</typeparam>
	/// <typeparam name="TBaseMessageType">The base message type.</typeparam>
	public interface IMessageHandler<in TMessageType, TBaseMessageType, in TMessageContext>
		where TMessageType : class, TBaseMessageType
	{
		/// <summary>
		/// Handles the message with <see cref="context"/> provided and correctly typed
		/// <see cref="message"/>.
		/// </summary>
		/// <param name="context">The message context.</param>
		/// <param name="message">The payload to handle.</param>
		Task HandleMessage(TMessageContext context, TMessageType message);
	}
}
