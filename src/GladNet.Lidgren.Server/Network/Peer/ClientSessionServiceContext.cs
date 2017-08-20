using GladNet.Engine.Common;
using GladNet.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Lidgren.Server
{
	public class ClientSessionServiceContext
	{
		public INetworkMessageRouterService SendService { get; }

		public INetworkMessageReceiver MessageReceiver { get; }

		public INetPeer ClientNetPeer { get; }

		public ClientSessionServiceContext(INetworkMessageRouterService sendService, INetworkMessageReceiver messageReceiver, INetPeer client)
		{
			if (sendService == null) throw new ArgumentNullException(nameof(sendService));
			if (messageReceiver == null) throw new ArgumentNullException(nameof(messageReceiver));
			if (client == null) throw new ArgumentNullException(nameof(client));

			SendService = sendService;
			MessageReceiver = messageReceiver;
			ClientNetPeer = client;
		}
	}
}
