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

		//https://github.com/HelloKitty/GladNet2.Specifications/blob/master/Routing/AUIDSpecification.md
		/// <summary>
		/// Application Unique ID (AUID) of the peer.
		/// </summary>
		int ConnectionID { get; }
	}
}
