using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public interface INetworkMessageSubcriptionBuilder<TNetworkMessageType>
		where TNetworkMessageType : INetworkMessage
	{
		INetworkMessageSubscriptionService Service { get; }
	}
}
