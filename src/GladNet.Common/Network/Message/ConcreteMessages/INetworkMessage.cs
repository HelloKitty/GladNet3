using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GladNet.Common
{
	/// <summary>
	/// Contract for a networked message. Provides the barest of function which exposes the <see cref="PacketPayload"/>
	/// of the message.
	/// </summary>
	public interface INetworkMessage
	{
		/// <summary>
		/// Represents the <see cref="PacketPayload"/> of the message.
		/// </summary>
		NetSendable<PacketPayload> Payload { get; }
	}
}
