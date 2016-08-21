using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Lidgren.Engine.Common
{
	/// <summary>
	/// Lidgren service contract for dispatching incoming network messages.
	/// </summary>
	public interface INetworkMessageDispatcher
	{
		/// <summary>
		/// Dispatches the incoming message.
		/// </summary>
		/// <param name="message"></param>
		void Dispatch(NetIncomingMessage message);
	}
}
