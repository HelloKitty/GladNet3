using GladNet.Engine.Common;
using GladNet.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Lidgren.Server.Unity
{
	public class ClientSessionServiceContext
	{
		public INetworkMessageRouterService SendService { get; }

		public INetworkMessageReceiver MessageReceiver { get; }

		public INetPeer ClientNetPeer { get; }

		public ClientSessionServiceContext(INetworkMessageRouterService sendService, INetworkMessageReceiver messageReceiver, INetPeer client)
		{
			SendService = sendService;
			MessageReceiver = messageReceiver;
			ClientNetPeer = client;
		}
	}
}
