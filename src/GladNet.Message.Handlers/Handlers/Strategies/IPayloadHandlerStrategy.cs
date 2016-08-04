using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladLive.Common
{
	/// <summary>
	/// Contract for being a strategy for handling <see cref="PacketPayload"/>s.
	/// </summary>
	/// <typeparam name="TSessionType">Session type that implements <see cref="INetPeer"/>.</typeparam>
	public interface IPayloadHandlerStrategy<TSessionType>
		where TSessionType : INetPeer
	{
		/// <summary>
		/// Attempts to handle the <see cref="PacketPayload"/> with static parameters.
		/// </summary>
		/// <param name="payload">Payload instance.</param>
		/// <param name="parameters">Parameters the message was sent with.</param>
		/// <param name="peer">Peer that is involved with the message.</param>
		/// <returns>True if the handler can handle the type of packet.</returns>
		bool TryProcessPayload(PacketPayload payload, IMessageParameters parameters, TSessionType peer);
	}
}
