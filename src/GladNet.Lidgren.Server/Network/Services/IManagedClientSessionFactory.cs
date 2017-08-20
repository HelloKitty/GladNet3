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
		/// <param name="sender"></param>
		/// <param name="details"></param>
		/// <param name="subService"></param>
		/// <param name="disconnectHandler"></param>
		/// <param name="routebackService"></param>
		/// <returns>A concrete peer instance or null if no peer could be created.</returns>
		ClientPeerSession CreateIncomingPeerSession(INetworkMessageRouterService sender, IConnectionDetails details, INetworkMessageSubscriptionService subService,
			IDisconnectionServiceHandler disconnectHandler, INetworkMessageRouteBackService routebackService);
	}
}
