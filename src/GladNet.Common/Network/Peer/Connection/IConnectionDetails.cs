using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;


namespace GladNet.Common
{
	/// <summary>
	/// Connection details describing the remote and local peer details.
	/// </summary>
	public interface IConnectionDetails
	{
		/// <summary>
		/// IPAddress of the remote peer.
		/// </summary>
		IPAddress RemoteIP { get; }

		/// <summary>
		/// Remote port of the peer.
		/// </summary>
		int RemotePort { get; }

		/// <summary>
		/// Local port the peer is connecting on.
		/// </summary>
		int LocalPort { get; }

		/// <summary>
		/// Connection ID of the peer. (unique per port)
		/// </summary>
		int ConnectionID { get; }
	}
}
