using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public interface IDisconnectionServiceHandler
	{
		event OnNetworkDisconnect DisconnectionEventHandler;

		bool isDisconnected { get; }

		void Disconnect();
	}
}
