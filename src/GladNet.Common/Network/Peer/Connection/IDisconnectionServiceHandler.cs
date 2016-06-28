using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	/// <summary>
	/// Service offering disconnection functionality.
	/// </summary>
	public interface IDisconnectionServiceHandler : IDisconnectable
	{
		/// <summary>
		/// Publisher of <see cref="OnNetworkDisconnect"/> events to subscribers.
		/// </summary>
		event OnNetworkDisconnect DisconnectionEventHandler;

		/// <summary>
		/// Indicates if the connection is disconnected.
		/// </summary>
		bool isDisconnected { get; }
	}
}
