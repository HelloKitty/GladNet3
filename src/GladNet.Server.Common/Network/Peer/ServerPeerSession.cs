using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet.Payload;

namespace GladNet.Server.Common
{
	/// <summary>
	/// Session for remote subserver peers that has the same functionality as <see cref="ClientPeerSession"/>s.
	/// </summary>
	public abstract class ServerPeerSession : ClientPeerSession
	{
		public ServerPeerSession(ILog logger, INetworkMessageRouterService sender, IConnectionDetails details, INetworkMessageSubscriptionService netMessageSubService,
			IDisconnectionServiceHandler disconnectHandler)
				: base(logger, sender, details, netMessageSubService, disconnectHandler)
		{

		}

		/// <summary>
		/// Called internally when a request is recieved from the remote peer.
		/// </summary>
		/// <param name="payload"><see cref="PacketPayload"/> sent by the peer.</param>
		/// <param name="parameters">Parameters the message was sent with.</param>
		protected override abstract void OnReceiveRequest(PacketPayload payload, IMessageParameters parameters);
	}
}
