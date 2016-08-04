using GladNet.Engine.Common;
using GladNet.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Message.Handlers
{
	/// <summary>
	/// Allows for registration of new <see cref="IMessageHandler{TPeerType}"/> instances.
	/// </summary>
	/// <typeparam name="TPeerType">Peer of the handler.</typeparam>
	/// <typeparam name="TNetworkMessageType">The network message Type of the handler.</typeparam>
	public interface IMessageHandlerRegistry<out TPeerType, out TNetworkMessageType>
		where TPeerType : INetPeer
		where TNetworkMessageType : INetworkMessage
	{
		/// <summary>
		/// Registers a new handler.
		/// </summary>
		/// <param name="messageHandler">Handler instance.</param>
		bool Register(IMessageHandler<TPeerType, TNetworkMessageType> messageHandler);
	}
}
