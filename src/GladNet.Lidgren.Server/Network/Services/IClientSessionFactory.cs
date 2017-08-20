using GladNet.Engine.Common;
using GladNet.Engine.Server;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Lidgren.Server
{
	public interface IClientSessionFactory
	{
		ClientPeerSession Create(IConnectionDetails connectionDetails, NetConnection connection);
	}
}
