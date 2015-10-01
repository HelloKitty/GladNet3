using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GladNet.Common
{
	/// <summary>
	/// Contract for types that provide <see cref="NetworkMessage"/> creation functionality.
	/// </summary>
	public interface INetworkMessageFactory
	{
		/// <summary>
		/// Creates an instance of a <see cref="NetworkMessage"/> type that corresponds to the <see cref="NetworkMessage.OperationType"/>
		/// and contains the <see cref="PacketPayload"/>/
		/// </summary>
		/// <param name="opType">Operation type of the <see cref="NetworkMessage"/>.</param>
		/// <param name="payload">Payload of the desired <see cref="NetworkMessage"/>.</param>
		/// <returns>Returns a unique <see cref="NetworkMessage"/> of the specificed <see cref="NetworkMessage.OperationType"/> containing the given <see cref="PacketPayload"/>.</returns>
		NetworkMessage Create(NetworkMessage.OperationType opType, PacketPayload payload);
	}
}
