using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GladNet.Common
{
	public interface INetPeer : INetworkMessageSender
	{
		IConnectionDetails PeerDetails { get; }

		bool CanSend(OperationType opType);
	}
}
