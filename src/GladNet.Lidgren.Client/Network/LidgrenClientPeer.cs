using GladNet.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using GladNet.Message;
using GladNet.Common;

namespace GladNet.Lidgren.Client
{
	public abstract class LidgrenClientPeer : ClientPeer
	{
		//Right now there isn't anything extra in a Lidgren peer. It needs to be used so that it
		//can be a future vector for delivering features to lidgren peers.

		public LidgrenClientPeer(ILog logger, INetworkMessagePayloadSenderService messageSender, IConnectionDetails details, INetworkMessageSubscriptionService subService, IDisconnectionServiceHandler disconnectHandler)
			: base(logger, messageSender, details, subService, disconnectHandler)
		{

		}
	}
}
