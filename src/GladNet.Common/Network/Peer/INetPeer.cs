using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GladNet.Common
{
	/// <summary>
	/// Implementer provides network peer services.
	/// </summary>
	public interface INetPeer : INetSender
	{
		NetStatus Status { get; }

		/// <summary>
		/// Provides details about the peer.
		/// </summary>
		IConnectionDetails PeerDetails { get; }

		/// <summary>
		/// Peer's service for sending network messages.
		/// </summary>
		INetworkMessageSender NetworkSendService { get; }
	}
}
