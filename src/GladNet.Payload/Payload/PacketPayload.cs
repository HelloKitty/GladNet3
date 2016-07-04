using GladNet.Serializer;
using System;

namespace GladNet.Payload
{
	/// <summary>
	/// Base-class for user-created wire-type data units.
	/// </summary>
	[CLSCompliant(true)]
	[GladNetSerializationContract]
	public abstract class PacketPayload
	{
		/// <summary>
		/// Public parameterless constructor for protobuf-net style
		/// serialization schemes.
		/// </summary>
		public PacketPayload()
		{
			
		}
	}
}
