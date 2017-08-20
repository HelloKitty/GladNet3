using GladNet.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Message.Handlers
{
	/// <summary>
	/// Combines strategies for payload handling in a chain of responsibility fashion.
	/// </summary>
	/// <typeparam name="TPeerType">The session type.</typeparam>
	public class MultipleChainResponsbilityMessageHandlerStrategy<TPeerType, TNetworkMessageType> : IMessageHandlerStrategy<TPeerType, TNetworkMessageType>
		where TPeerType : INetPeer
		where TNetworkMessageType : INetworkMessage
	{
		/// <summary>
		/// Combined collection of enumerable strategies for handling <typeparamref name="TNetworkMessageType"/>s.
		/// </summary>
		private IEnumerable<IMessageHandlerStrategy<TPeerType, TNetworkMessageType>> strategyChain { get; }

		public MultipleChainResponsbilityMessageHandlerStrategy(IEnumerable<IMessageHandlerStrategy<TPeerType, TNetworkMessageType>> strategies)
		{
			if (strategies == null) throw new ArgumentNullException(nameof(strategies), $"Chain handler must have non-null collection of strategies to chain over.");

			strategyChain = strategies;
		}

		public MultipleChainResponsbilityMessageHandlerStrategy(params IMessageHandlerStrategy<TPeerType, TNetworkMessageType>[] strategies)
		{
			if (strategies == null) throw new ArgumentNullException(nameof(strategies), $"Chain handler must have non-null collection of strategies to chain over.");

			strategyChain = strategies;
		}

		/// <summary>
		/// Attempts to handle the <typeparamref name="TNetworkMessageType"/>.
		/// </summary>
		/// <param name="message">Network messages instance.</param>
		/// <param name="parameters">Parameters the message was sent with.</param>
		/// <param name="peer">Peer that is involved with the message.</param>
		/// <returns>True if the handler can handle the message.</returns>
		public bool TryProcessMessage(TNetworkMessageType message, IMessageParameters parameters, TPeerType peer)
		{
			//Defer the request to each strat and if one handles it properly then we stop chaining
			foreach (IMessageHandlerStrategy<TPeerType, TNetworkMessageType> s in strategyChain)
				if (s.TryProcessMessage(message, parameters, peer))
					return true;

			return false;
		}
	}
}
