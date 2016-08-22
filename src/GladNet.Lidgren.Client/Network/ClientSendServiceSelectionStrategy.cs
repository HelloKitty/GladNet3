using GladNet.Lidgren.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GladNet.Engine.Common;

namespace GladNet.Lidgren.Client
{
	public class ClientSendServiceSelectionStrategy : ISendServiceSelectionStrategy
	{
		private INetworkMessageRouterService clientMessageRouterService { get; }

		public ClientSendServiceSelectionStrategy(INetworkMessageRouterService routerService)
		{
			if (routerService == null)
				throw new ArgumentNullException(nameof(routerService), $"Provided {nameof(INetworkMessageRouterService)} cannot be null.");

			clientMessageRouterService = routerService; 
		}

		public INetworkMessageRouterService GetRouterService(int connectionId)
		{
			//Doesn't matter what the connection id is. A client only has one send service
			return clientMessageRouterService;
		}
	}
}
