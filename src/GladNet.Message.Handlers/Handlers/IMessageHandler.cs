using GladNet.Engine.Common;
using GladNet.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Message.Handlers
{
	/// <summary>
	/// Service tries to handle a <see cref="INetworkMessage"/>.
	/// </summary>
	/// <typeparam name="TPeerType">Type of the peer.</typeparam>
	/// <typeparam name="TNetworkMessageType">Type of the <see cref="INetworkMessage"/>.</typeparam>
	public interface IMessageHandler<in TPeerType, in TNetworkMessageType>
		where TPeerType : INetPeer where TNetworkMessageType : INetworkMessage
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
