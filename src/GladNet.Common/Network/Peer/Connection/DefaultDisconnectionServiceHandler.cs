using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common.Network
{
	//Stole this implementation from GladNet.PhotonServer's adapter.
	//Should serve well as a simple implementation in-lieu of custom implementations
	//for specific net libs.
	/// <summary>
	/// Default simple implementation of the <see cref="IDisconnectionServiceHandler"/> interface.
	/// </summary>
	public class DefaultDisconnectionServiceHandler : IDisconnectionServiceHandler
	{
		//TODO: Make this thread safe

		/// <summary>
		/// Indicates if the connection is disconnected.
		/// </summary>
		public bool isDisconnected { get; private set; }

		/// <summary>
		/// Publisher of <see cref="OnNetworkDisconnect"/> events to subscribers.
		/// </summary>
		public event OnNetworkDisconnect DisconnectionEventHandler;

		/// <summary>
		/// Creates a new adapter for the <see cref="IDisconnectionServiceHandler"/> interface.
		/// </summary>
		public DefaultDisconnectionServiceHandler()
		{
			isDisconnected = false;
		}

		/// <summary>
		/// Disconnects the connection.
		/// </summary>
		public void Disconnect()
		{
			isDisconnected = true;

			//Call subscribers for the disconnection event.
			if (DisconnectionEventHandler != null)
				DisconnectionEventHandler.Invoke();
		}
	}
}
