using System;
using System.Collections.Generic;
using System.Text;

namespace GladNet
{
	/// <summary>
	/// Contract for a type that can construct
	/// packets.
	/// <typeparam name="TPayloadConstraintType">This is the contraint type required by the factory to act on the payload.</typeparam>
	/// </summary>
	public interface IPacketHeaderFactory<in TPayloadConstraintType>
	{
		/// <summary>
		/// Creates a new <see cref="IPacketHeader"/> for the provided payload data.
		/// Uses the original payload instance and the serialized payload to build the packet.
		/// </summary>
		/// <param name="payload">The payload that is being sent.</param>
		/// <param name="serializedPayloadData">The serialized payload data for the payload.</param>
		/// <returns>A new packet header for the payload.</returns>
		IPacketHeader Create<TPayloadType>(TPayloadType payload, byte[] serializedPayloadData)
			where TPayloadType : TPayloadConstraintType;
	}
}
