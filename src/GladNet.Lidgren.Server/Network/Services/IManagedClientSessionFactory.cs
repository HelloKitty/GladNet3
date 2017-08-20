using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GladNet.Engine.Common;
using GladNet.Engine.Server;

namespace GladNet.Lidgren.Server
{
	public interface IManagedClientSessionFactory
	{
		/// <summary>
		/// Create the concrete managed peer from the provided services.
		/// </summary>
		/// <param name="messageSender"></param>
		/// <param name="details"></param>
		/// <param name="subService"></param>
		/// <param name="disconnectHandler"></param>
		/// <returns>A concrete peer instance or null if no peer could be created.</returns>
		ClientPeerSession CreateIncomingPeerSession(INetworkMessagePayloadSenderService messageSender, IConnectionDetails details, INetworkMessageSubscriptionService subService,
			IDisconnectionServiceHandler disconnectHandler);
	}
}
