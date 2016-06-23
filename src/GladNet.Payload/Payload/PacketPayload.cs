using GladNet.Serializer;

namespace GladNet.Common
{
	/// <summary>
	/// Base-class for user-created wire-type data units.
	/// </summary>
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
