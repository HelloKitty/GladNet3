using System;
using System.Collections.Generic;
using System.Text;

namespace GladNet
{
	/// <summary>
	/// Enumeration of connection status.
	/// </summary>
	public enum ConnectionStatus : int
	{
		/// <summary>
		/// Unknown or invaid state.
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// Indicates the connection is connecting.
		/// </summary>
		Connecting = 1,

		/// <summary>
		/// A valid connected state.
		/// </summary>
		Connected = 2,

		/// <summary>
		/// Indicates that the client has begun disconnecting.
		/// </summary>
		Disconnecting = 3,

		/// <summary>
		/// Indicates that the session is disconnected.
		/// </summary>
		Disconnected = 4
	}
}
