using GladNet.Payload;
using GladNet.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GladNet.Common
{
	/// <summary>
	/// Contract for a networked message. Provides the barest of function which exposes the <see cref="NetSendable"/> <see cref="PacketPayload"/>
	/// of the message.
	/// </summary>
	public interface INetworkMessage : ISerializationVisitable
	{
		/// <summary>
		/// The payload of a <see cref="INetworkMessage"/>. Can be sent accross a network.
		/// <see cref="NetSendable"/> enforces its wire readyness.
		/// </summary>
		NetSendable<PacketPayload> Payload { get; }
	}
}
