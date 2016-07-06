using GladNet.Common;
using GladNet.Payload;
using GladNet.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GladNet.Message
{
	/// <summary>
	/// Contract for a networked message. Provides the barest of function which exposes the <see cref="NetSendable"/> <see cref="PacketPayload"/>
	/// of the message.
	/// </summary>
	public interface INetworkMessage : ISerializationVisitable, IPayloadContainer
	{

	}
}
