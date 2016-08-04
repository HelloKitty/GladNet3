using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladLive.Common
{
	/// <summary>
	/// Allows for registration of new <see cref="IPayloadHandler{TPeerType}"/> instances.
	/// </summary>
	/// <typeparam name="TPeerType">Peer of the handler.</typeparam>
	public interface IPayloadHandlerRegistry<in TPeerType>
		where TPeerType : INetPeer
	{
		/// <summary>
		/// Registers a new handler.
		/// </summary>
		/// <typeparam name="THandlerPeerType">The type of peer of the handler.</typeparam>
		/// <typeparam name="TPayloadType">Type of the payload handled.</typeparam>
		/// <param name="payloadHandler">Handler instance.</param>
		bool Register<THandlerPeerType, TPayloadType>(IPayloadHandler<THandlerPeerType, TPayloadType> payloadHandler)
			where TPayloadType : PacketPayload where THandlerPeerType : TPeerType;
	}
}
