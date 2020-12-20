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
		//This is explicitly implemented because if someone has a reference to this type directly we'd rather they see the specific API.
		/// <inheritdoc />
		Task IMessageHandler<TBaseMessageType, TMessageContext>.HandleMessageAsync(TMessageContext context, TBaseMessageType message, CancellationToken token = default)
		{
			//Because this method is implemented expliclity we don't need to ASSERT that this isn't
			//MessageType == BaseMessageType and infinite recurring. Meaning equivalent type parameters
			//are valid and safe now. Default handler works this way!
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

		/// <inheritdoc />
		public void BindTo(ITypeBinder<IMessageHandler<TBaseMessageType, TMessageContext>, TBaseMessageType> bindTarget)
		{
			if (bindTarget == null) throw new ArgumentNullException(nameof(bindTarget));

			bindTarget.Bind<TMessageType>(this);

			//Allow implementers to bind this to other types too
			ExtraBindings(bindTarget);
		}

		/// <summary>
		/// Cam be overriden to bind this <see cref="IMessageHandler{TMessageType,TMessageContext}"/> to multiple message types.
		/// </summary>
		/// <param name="bindTarget">The binding target to bind this handler to.</param>
		protected virtual void ExtraBindings(ITypeBinder<IMessageHandler<TBaseMessageType, TMessageContext>, TBaseMessageType> bindTarget)
		{
			if (bindTarget == null) throw new ArgumentNullException(nameof(bindTarget));

			//Example: bindTarget.Bind<Derp>(this);
		}
	}
}
