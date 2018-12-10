using System;
using System.Collections.Generic;
using System.Text;

namespace GladNet
{
	/// <summary>
	/// Disconnected network exception.
	/// </summary>
	public sealed class NetworkDisconnectedException : InvalidOperationException
	{
		/// <summary>
		/// Creates a new disconnected exception that tells
		/// up the stack that the network is disconnected and operations can't act on it.
		/// </summary>
		public NetworkDisconnectedException()
			: base("The network is disconnected.")
		{
			
		}

		public NetworkDisconnectedException(string message)
			: base(message)
		{
			
		}
	}
}
