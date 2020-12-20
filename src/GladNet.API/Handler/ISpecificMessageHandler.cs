using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
	public interface ISpecificMessageHandler<in TMessageType, in TBaseMessageType, in TMessageContext> : IMessageHandler<TBaseMessageType, TMessageContext>
		where TMessageType : class, TBaseMessageType 
		where TBaseMessageType : class
	{

	}
}
