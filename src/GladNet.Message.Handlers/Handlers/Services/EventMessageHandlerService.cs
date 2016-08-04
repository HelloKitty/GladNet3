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
	/// <see cref="INetworkMessage"/> handler service that handles events based on the
	/// strategy provided.
	/// </summary>
	/// <typeparam name="TPeerType">Type of the peer.</typeparam>
	public class EventMessageHandlerService<TPeerType> : IEventMessageHandlerService<TPeerType>
		where TPeerType : INetPeer
	{
		/// <summary>
		/// Strategy for how to handle incoming <see cref="IEventMessage"/>
		/// </summary>
		private IMessageHandlerStrategy<TPeerType, IEventMessage> handlerStrat { get; }

		public EventMessageHandlerService(IMessageHandlerStrategy<TPeerType, IEventMessage> strat)
		{
			Throw<ArgumentNullException>.If.IsNull(strat)?.Now(nameof(strat), $"Service must have non-null handler strategy.");

			handlerStrat = strat;
		}

		/// <summary>
		/// Attempts to handle the <see cref="IEventMessage"/>
		/// </summary>
		/// <param name="message">Network messages instance.</param>
		/// <param name="parameters">Parameters the message was sent with.</param>
		/// <param name="peer">Peer that is involved with the message.</param>
		/// <returns>True if the handler can handle the message.</returns>
		public bool TryProcessMessage(IEventMessage message, IMessageParameters parameters, TPeerType peer)
		{
			return handlerStrat.TryProcessMessage(message, parameters, peer);
		}
	}
}
