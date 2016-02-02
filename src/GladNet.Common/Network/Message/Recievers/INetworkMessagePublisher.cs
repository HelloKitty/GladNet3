using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public interface INetworkMessagePublisher
	{
		OnNetworkEventMessage EventPublisher { get; }
		OnNetworkRequestMessage RequestPublisher { get; }
		OnNetworkResponseMessage ResponsePublisher { get; }
		OnNetworkStatusMessage StatusPublisher { get; }
	}
}
