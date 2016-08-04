using Easyception;
using GladNet.Engine.Common;
using GladNet.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Message.Handlers
{
	/// <summary>
	/// Provides chain of responsibility semantics as a strategy for payload handling.
	/// </summary>
	/// <typeparam name="TPeerType">Session type to handle.</typeparam>
	public class ChainMessageHandlerStrategy<TPeerType, TNetworkMessageType> : IMessageHandlerStrategy<TPeerType, TNetworkMessageType>, IMessageHandlerRegistry<TPeerType, TNetworkMessageType>
		where TPeerType : INetPeer
		where TNetworkMessageType : INetworkMessage
	{
		/// <summary>
		/// Collection of handles to chain over.
		/// </summary>
		private IList<IMessageHandler<TPeerType, TNetworkMessageType>> handlers { get; }

		public ChainMessageHandlerStrategy()
		{
			handlers = new List<IMessageHandler<TPeerType, TNetworkMessageType>>();
		}

		public ChainMessageHandlerStrategy(IEnumerable<IMessageHandler<TPeerType, TNetworkMessageType>> handlersToChain)
		{
			Throw<ArgumentNullException>.If.IsNull(handlersToChain)?.Now(nameof(handlersToChain), $"Chain handler must have non-null collection of handlers to chain over.");

			handlers = new List<IMessageHandler<TPeerType, TNetworkMessageType>>(handlersToChain);
		}

		/// <summary>
		/// Attempts to handle the <typeparamref name="TNetworkMessageType"/>.
		/// </summary>
		/// <param name="message">The message instance.</param>
		/// <param name="parameters">Parameters the message was sent with.</param>
		/// <param name="peer">Peer that is involved with the message.</param>
		/// <returns>True if the handler can handle the message.</returns>
		public virtual bool TryProcessMessage(TNetworkMessageType message, IMessageParameters parameters, TPeerType peer)
		{
			bool result = false;

			foreach(IMessageHandler<TPeerType, TNetworkMessageType> h in handlers)
			{
				result = h.TryProcessMessage(message, parameters, peer) || result;

				//Added consumption to the chain making the payloads handleable by only a single handler
				if (result)
					return true;
			}

			return result;
		}

		public bool Register(IMessageHandler<TPeerType, TNetworkMessageType> messageHandler)
		{
			//Adds the handler to the collection
			//In the future we can do fancier things like checking to see if it has already been registered
			//We can also maybe lock to prepare for multithreading
			if (messageHandler != null)
			{
				handlers.Add(messageHandler);
				return true;
			}

			return false;
		}
	}
}
