using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet
{
	/// <summary>
	/// Message handling service that aggregates handlers
	/// and manages the try dispatching of them and accepts a default
	/// handler to fall back to if it fails.
	/// </summary>
	/// <typeparam name="TIncomingPayloadType"></typeparam>
	/// <typeparam name="TOutgoingPayloadType"></typeparam>
	public sealed class MessageHandlerService<TIncomingPayloadType, TOutgoingPayloadType>: MessageHandlerService<TIncomingPayloadType, TOutgoingPayloadType, IPeerMessageContext<TOutgoingPayloadType>>, IPeerMessageHandler<TIncomingPayloadType, TOutgoingPayloadType>
		where TIncomingPayloadType : class
		where TOutgoingPayloadType : class
	{
		/// <inheritdoc />
		public MessageHandlerService(IEnumerable<IPeerMessageHandler<TIncomingPayloadType, TOutgoingPayloadType, IPeerMessageContext<TOutgoingPayloadType>>> managedHandlers, IPeerPayloadSpecificMessageHandler<TIncomingPayloadType, TOutgoingPayloadType, IPeerMessageContext<TOutgoingPayloadType>> defaultMessageHandler) 
			: base(managedHandlers, defaultMessageHandler)
		{

		}
	}

	/// <summary>
	/// Message handling service that aggregates handlers
	/// and manages the try dispatching of them and accepts a default
	/// handler to fall back to if it fails.
	/// </summary>
	/// <typeparam name="TIncomingPayloadType"></typeparam>
	/// <typeparam name="TOutgoingPayloadType"></typeparam>
	/// <typeparam name="TPeerContextType"></typeparam>
	public class MessageHandlerService<TIncomingPayloadType, TOutgoingPayloadType, TPeerContextType> : IPeerMessageHandler<TIncomingPayloadType, TOutgoingPayloadType, TPeerContextType>
		where TIncomingPayloadType : class
		where TOutgoingPayloadType : class
		where TPeerContextType : IPeerMessageContext<TOutgoingPayloadType>
	{
		/// <summary>
		/// The handlers this service will try to dispatch to.
		/// </summary>
		private IPeerMessageHandler<TIncomingPayloadType, TOutgoingPayloadType, TPeerContextType>[] ManagedHandlers { get; }

		/// <summary>
		/// The optional default message handler to fall back on
		/// if no handler accepts the incoming message.
		/// </summary>
		private IPeerPayloadSpecificMessageHandler<TIncomingPayloadType, TOutgoingPayloadType, TPeerContextType> DefaultMessageHandler { get; }

		/// <inheritdoc />
		public MessageHandlerService(IEnumerable<IPeerMessageHandler<TIncomingPayloadType, TOutgoingPayloadType, TPeerContextType>> managedHandlers, IPeerPayloadSpecificMessageHandler<TIncomingPayloadType, TOutgoingPayloadType, TPeerContextType> defaultMessageHandler)
		{
			if(managedHandlers == null) throw new ArgumentNullException(nameof(managedHandlers));

			ManagedHandlers = managedHandlers.ToArray();

			//Default handler can be null.
			DefaultMessageHandler = defaultMessageHandler;
		}

		/// <inheritdoc />
		public bool CanHandle(NetworkIncomingMessage<TIncomingPayloadType> message)
		{
			//We can always handle messages
			return true;
		}

		/// <inheritdoc />
		public async Task<bool> TryHandleMessage(TPeerContextType context, NetworkIncomingMessage<TIncomingPayloadType> message)
		{
			//TODO: What should we do about exceptions?
			//When a message comes in we need to try to dispatch it to all handlers
			foreach(IPeerMessageHandler<TIncomingPayloadType, TOutgoingPayloadType, TPeerContextType> handler in ManagedHandlers)
			{
				//If we found a handler that handled it we should stop trying to handle it and return true
				if(handler.CanHandle(message))
					return await handler.TryHandleMessage(context, message)
						.ConfigureAwait(false);
			}

			DefaultMessageHandler?.HandleMessage(context, message.Payload)?
				.ConfigureAwait(true);

			return false;
		}
	}
}
