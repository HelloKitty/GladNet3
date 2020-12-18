using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladNet;

namespace GladNet
{
	/// <summary>
	/// Contract for services that provide the ability to send messages.
	/// </summary>
	/// <typeparam name="TMessageBaseType">The message's base type that is sent.</typeparam>
	public interface IMessageSendService<in TMessageBaseType>
		where TMessageBaseType : class
	{
		/// <summary>
		/// Sends the provided <see cref="message"/>
		/// </summary>
		/// <typeparam name="TMessageType">The type of message.</typeparam>
		/// <param name="message">The message to send.</param>
		/// <returns>Indicates the result of the send message operation.</returns>
		Task<SendResult> SendMessage<TMessageType>(TMessageType message)
			where TMessageType : class, TMessageBaseType;
	}
}
