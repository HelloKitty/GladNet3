using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public interface INetworkMessageSubscriptionService
	{
		void SubscribeToRequests(OnNetworkRequestMessage subscriber);
		void SubscribeToEvents(OnNetworkEventMessage subscriber);
		void SubscribeToResponses(OnNetworkResponseMessage subscriber);
		void SubscribeToStatusChanges(OnNetworkStatusMessage subscriber);
	}
}
