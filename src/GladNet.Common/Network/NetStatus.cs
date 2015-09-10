using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	/// <summary>
	/// Represents the network status of a connection.
	/// </summary>
	public enum NetStatus : byte
	{
		Connecting,
		Connected,
		EncryptionEstablished,
		Disconnecting,
		Disconnected
	}
}
