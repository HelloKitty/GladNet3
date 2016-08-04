using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladLive.Common
{
	/// <summary>
	/// Service tries to handle a <typeparamref name="TPayloadType"/>.
	/// </summary>
	/// <typeparam name="TPeerType">Type of the peer.</typeparam>
	/// <typeparam name="TPayloadType">Type of the payload it handles.</typeparam>
	public interface IPayloadHandler<in TPeerType, TPayloadType> : IPayloadHandler<TPeerType>
		where TPeerType : INetPeer where TPayloadType : PacketPayload
	{
		/// <summary>
		/// Attempts to handle the <typeparamref name="TPayloadType"/> with static parameters.
		/// </summary>
		/// <typeparam name="TPayloadType">Payload type.</typeparam>
		/// <param name="payload">Payload instance.</param>
		/// <param name="parameters">Parameters the message was sent with.</param>
		/// <param name="peer">Peer that is involved with the message.</param>
		/// <returns>True if the handler can handle the type of packet.</returns>
		bool TryProcessPayload(TPayloadType payload, IMessageParameters parameters, TPeerType peer);
	}

	/// <summary>
	/// Service tries to handle a <see cref="PacketPayload"/>.
	/// </summary>
	/// <typeparam name="TPeerType">Type of the peer.</typeparam>
	public interface IPayloadHandler<in TPeerType>
		where TPeerType : INetPeer
	{
		/// <summary>
		/// Attempts to handle the <typeparamref name="TPayloadType"/> with static parameters.
		/// </summary>
		/// <typeparam name="TPayloadType">Payload type.</typeparam>
		/// <param name="payload">Payload instance.</param>
		/// <param name="parameters">Parameters the message was sent with.</param>
		/// <param name="peer">Peer that is involved with the message.</param>
		/// <returns>True if the handler can handle the type of packet.</returns>
		bool TryProcessPayload(PacketPayload payload, IMessageParameters parameters, TPeerType peer);
	}
}
