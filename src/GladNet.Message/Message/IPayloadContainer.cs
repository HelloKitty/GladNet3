using GladNet.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Message
{
	/// <summary>
	/// Implementer exposes a <see cref="NetSendable{PacketPayload}"/>
	/// </summary>
	public interface IPayloadContainer
	{
		/// <summary>
		/// The payload of a <see cref="INetworkMessage"/>. Can be sent accross a network.
		/// <see cref="NetSendable"/> enforces its wire readyness.
		/// </summary>
		NetSendable<PacketPayload> Payload { get; }
	}
}
