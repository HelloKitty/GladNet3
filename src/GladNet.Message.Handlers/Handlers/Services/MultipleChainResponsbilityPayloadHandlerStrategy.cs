using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladLive.Common
{
	/// <summary>
	/// Combines strategies for payload handling in a chain of responsibility fashion.
	/// </summary>
	/// <typeparam name="TSessionType">The session type.</typeparam>
	public class MultipleChainResponsbilityPayloadHandlerStrategy<TSessionType> : IPayloadHandlerStrategy<TSessionType>
		where TSessionType : INetPeer
	{
		/// <summary>
		/// Combined collection of enumerable strategies for handling <see cref="PacketPayload"/>s.
		/// </summary>
		private IEnumerable<IPayloadHandlerStrategy<TSessionType>> strategyChain { get; }

		public MultipleChainResponsbilityPayloadHandlerStrategy(IEnumerable<IPayloadHandlerStrategy<TSessionType>> strategies)
		{
			strategyChain = strategies;
		}

		public MultipleChainResponsbilityPayloadHandlerStrategy(params IPayloadHandlerStrategy<TSessionType>[] strategies)
		{
			strategyChain = strategies;
		}

		/// <summary>
		/// Attempts to handle the <see cref="PacketPayload"/>.
		/// </summary>
		/// <param name="payload">Payload instance.</param>
		/// <param name="parameters">Parameters the message was sent with.</param>
		/// <param name="peer">Peer that is involved with the message.</param>
		/// <returns>True if the handler can handle the type of packet.</returns>
		public bool TryProcessPayload(PacketPayload payload, IMessageParameters parameters, TSessionType peer)
		{
			//Defer the request to each strat and if one handles it properly then we stop chaining
			foreach (IPayloadHandlerStrategy<TSessionType> s in strategyChain)
				if (s.TryProcessPayload(payload, parameters, peer))
					return true;

			return false;
		}
	}
}
