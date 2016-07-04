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
	public interface INetworkMessage : ISerializationVisitable, IPayloadContainer
#if !ENDUSER //EndUser doesn't need message routing functionality
		, IRoutableMessage
#endif
	{
#if !ENDUSER
		/// <summary>
		/// Exports the internal routing data to the target <see cref="IRoutableMessage"/>
		/// parameter <paramref name="message"/>.
		/// </summary>
		/// <param name="message"></param>
		void ExportRoutingDataTo(IRoutableMessage message);
#endif
	}
}
