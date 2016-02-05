using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GladNet.Common
{
	/// <summary>
	/// Implementer provides network peer services.
	/// </summary>
	public interface INetPeer
	{
		/// <summary>
		/// Provides details about the peer.
		/// </summary>
		IConnectionDetails PeerDetails { get; }

		/// <summary>
		/// Indicates if the <see cref="OperationType"/> can be sent with this peer.
		/// </summary>
		/// <param name="opType"><see cref="OperationType"/> to check.</param>
		/// <returns>True if the peer can see the <paramref name="opType"/>.</returns>
		bool CanSend(OperationType opType);

		/// <summary>
		/// Peer's service for sending network messages.
		/// </summary>
		INetworkMessageSender NetworkSendService { get; }
	}
}
