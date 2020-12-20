using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace GladNet
{
	/// <summary>
	/// Basic message handling service that maps incoming message types to <see cref="IMessageHandler{TMessageType,TMessageContext}"/>'s
	/// that are bound and can handle that specific type.
	/// </summary>
	/// <typeparam name="TMessageType"></typeparam>
	/// <typeparam name="TMessageContext"></typeparam>
	public sealed class DefaultMessageHandlerService<TMessageType, TMessageContext> 
		: IMessageHandlerService<TMessageType, TMessageContext>, ITypeBinder<IMessageHandler<TMessageType, TMessageContext>, TMessageType>
		where TMessageType : class
	{
		/// <summary>
		/// Internal async syncronization object.
		/// This exists because you may want to bind and unbind handlers during runtime.
		/// </summary>
		private AsyncReaderWriterLock SyncObj { get; } = new AsyncReaderWriterLock();

		/// <summary>
		/// Internal routing map that maps message <see cref="Type"/> to <see cref="IMessageHandler{TMessageType,TMessageContext}"/> instance.
		/// </summary>
		private Dictionary<Type, IMessageHandler<TMessageType, TMessageContext>> HandlerRouteMap { get; } = new Dictionary<Type, IMessageHandler<TMessageType, TMessageContext>>();

		/// <inheritdoc />
		public async Task<bool> HandleMessageAsync(TMessageContext context, TMessageType message, CancellationToken token = default)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));
			if (message == null) throw new ArgumentNullException(nameof(message));

			IMessageHandler<TMessageType, TMessageContext> handler;
			using (await SyncObj.ReaderLockAsync())
			{
				//TODO: Should we log?
				if (!HandlerRouteMap.ContainsKey(message.GetType()))
				{
					handler = HandlerRouteMap[typeof(TMessageType)];
				}
				else
					//Route to a handler that matches the message type.
					handler = HandlerRouteMap[message.GetType()];
			}

			//Possible there is no bound handler for this message type.
			if (handler == null)
				return false;

			//We want to call this OUTSIDE the lock, no reason to hold the lock for this
			await handler.HandleMessageAsync(context, message, token);
			return true;
		}

		/// <inheritdoc />
		public bool Bind<TBindType>(IMessageHandler<TMessageType, TMessageContext> target)
			where TBindType : TMessageType
		{
			using (SyncObj.WriterLock())
			{
				HandlerRouteMap[typeof(TBindType)] = target;
			}

			return true;
		}
	}
}
