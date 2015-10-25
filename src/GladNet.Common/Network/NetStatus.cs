using GladNet.Serializer;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace GladNet.Common
{

	//We suppress this because this is going over the wire. 1 byte is far better.
	[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
	/// <summary>
	/// Represents the network status of a connection.
	/// </summary>
	[GladNetSerializationContract]
	public enum NetStatus : byte
	{
		Connecting,
		Connected,
		EncryptionEstablished,
		Disconnecting,
		Disconnected
	}
}
