using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GladNet
{
	/// <summary>
	/// Base implementation of a <see cref="IMessageHandler{TMessageType,TMessageContext}"/> that supports handling
	/// a specific message type that derives from the base message type.
	/// </summary>
	/// <typeparam name="TMessageType">The exact type of message to handle.</typeparam>
	/// <typeparam name="TBaseMessageType">The base message type.</typeparam>
	/// <typeparam name="TMessageContext">The type of the message context.</typeparam>
	public abstract class BaseSpecificMessageHandler<TMessageType, TBaseMessageType, TMessageContext> 
		: ISpecificMessageHandler<TMessageType, TBaseMessageType, TMessageContext> 
		where TMessageType : class, TBaseMessageType 
		where TBaseMessageType : class
	{
		static BaseSpecificMessageHandler()
		{
			if (typeof(TMessageType) == typeof(TBaseMessageType))
				throw new InvalidOperationException($"Cannot have a {nameof(BaseSpecificMessageHandler<TMessageType, TBaseMessageType, TMessageContext>)} with identical {nameof(TMessageType)} and {nameof(TBaseMessageType)}.");
		}

		//This is explicitly implemented because if someone has a reference to this type directly we'd rather they see the specific API.
		/// <inheritdoc />
		Task IMessageHandler<TBaseMessageType, TMessageContext>.HandleMessageAsync(TMessageContext context, TBaseMessageType message, CancellationToken token = default)
		{
			return HandleMessageAsync(context, (TMessageType)message, token);
		}

		/// <summary>
		/// Handles the message with <see cref="context"/> provided and correctly typed
		/// <see cref="message"/>.
		/// </summary>
		/// <param name="context">The message context.</param>
		/// <param name="message">The payload to handle.</param>
		/// <param name="token">The cancel token for the handle operation.</param>
		public abstract Task HandleMessageAsync(TMessageContext context, TMessageType message, CancellationToken token = default);
	}
}
