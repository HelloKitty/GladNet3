using GladNet.Common;
using GladNet.Message;
using GladNet.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Lidgren.Engine.Common
{
	/// <summary>
	/// Factory service for creating <see cref="NetworkMessage"/> instances.
	/// </summary>
	public interface INetworkMessageFactory
	{
		/// <summary>
		/// Creates a new <see cref="INetworkMessage"/>
		/// </summary>
		/// <param name="opType">Operation type of the message.</param>
		/// <param name="payload">Payload for the <see cref="INetworkMessage"/>.</param>
		/// <returns>A new non-null <see cref="INetworkMessage"/>.</returns>
		INetworkMessage Create(OperationType opType, PacketPayload payload);
	}
}
