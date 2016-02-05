using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logging.Services;

namespace GladNet.Server.Common
{
	public abstract class ServerPeer : Peer
	{
		protected ServerPeer(ILogger logger, INetworkMessageSender messageSender, IConnectionDetails details) 
			: base(logger, messageSender, details)
		{

		}
	}
}
