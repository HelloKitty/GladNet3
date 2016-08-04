using GladNet.Engine.Common;
using GladNet.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Message.Handlers
{
	/// <summary>
	/// Contract for being a strategy for handling <typeparamref name="TNetworkMessageType"/>
	/// </summary>
	/// <typeparam name="TPeerType">Peer type that implements <see cref="INetPeer"/>.</typeparam>
	/// <typeparam name="TNetworkMessageType">The network message type.</typeparam>
	public interface IMessageHandlerStrategy<in TPeerType, in TNetworkMessageType>
		where TPeerType : INetPeer
		where TNetworkMessageType : INetworkMessage
	{
		/// <summary>
		/// Attempts to handle the <typeparamref name="TNetworkMessageType"/>.
		/// </summary>
		/// <param name="message">Network messages instance.</param>
		/// <param name="parameters">Parameters the message was sent with.</param>
		/// <param name="peer">Peer that is involved with the message.</param>
		/// <returns>True if the handler can handle the message.</returns>
		bool TryProcessMessage(TNetworkMessageType message, IMessageParameters parameters, TPeerType peer);
	}
}
