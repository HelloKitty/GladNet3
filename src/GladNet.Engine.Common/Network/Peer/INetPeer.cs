using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GladNet.Engine.Common
{
	/// <summary>
	/// Implementer provides network peer services.
	/// </summary>
	public interface INetPeer : INetSender
	{
		/// <summary>
		/// Indicates the Network Status of the current <see cref="INetPeer"/>.
		/// </summary>
		NetStatus Status { get; }

		/// <summary>
		/// Provides details about the peer.
		/// </summary>
		IConnectionDetails PeerDetails { get; }

		/// <summary>
		/// Peer's service for sending network messages.
		/// </summary>
		INetworkMessageRouterService NetworkSendService { get; }
	}
}
