using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logging.Services;

namespace GladNet.Server.Common
{
	/// <summary>
	/// Session for remote subserver peers that has the same functionality as <see cref="ClientPeerSession"/>s.
	/// </summary>
	public abstract class ServerPeerSession : ClientPeerSession
	{
		public ServerPeerSession(ILogger logger, INetworkMessageSender sender, IConnectionDetails details, INetworkMessageSubscriptionService netMessageSubService) 
			: base(logger, sender, details, netMessageSubService)
		{

		}

		/// <summary>
		/// Called internally when a request is recieved from the remote peer.
		/// </summary>
		/// <param name="message"><see cref="IRequestMessage"/> sent by the peer.</param>
		/// <param name="parameters">Parameters the message was sent with.</param>
		protected override abstract void OnReceiveRequest(IRequestMessage message, IMessageParameters parameters);
	}
}
