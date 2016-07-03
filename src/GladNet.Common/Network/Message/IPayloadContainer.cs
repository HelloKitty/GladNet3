using GladNet.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	/// <summary>
	/// Implementer exposes a <see cref="PacketPayload"/>
	/// </summary>
	public interface IPayloadContainer
	{
		/// <summary>
		/// Instance of the <see cref="PacketPayload"/> in the container.
		/// </summary>
		PacketPayload Payload { get; }
	}
}
