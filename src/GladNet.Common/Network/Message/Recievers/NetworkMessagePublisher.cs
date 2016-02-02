using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public class NetworkMessagePublisher : INetworkMessageReceiver, INetworkMessageSubscriptionService, INetworkMessagePublisher
	{
		public OnNetworkEventMessage EventPublisher { get; private set; }

		public OnNetworkRequestMessage RequestPublisher { get; private set; }

		public OnNetworkResponseMessage ResponsePublisher { get; private set; }

		public OnNetworkStatusMessage StatusPublisher { get; private set; }

		public void OnNetworkMessageReceive(IEventMessage message, IMessageParameters parameters)
		{
			throw new NotImplementedException();
		}

		public void OnNetworkMessageReceive(IResponseMessage message, IMessageParameters parameters)
		{
			throw new NotImplementedException();
		}

		public void OnNetworkMessageReceive(IRequestMessage message, IMessageParameters parameters)
		{
			throw new NotImplementedException();
		}

		public void OnStatusChanged(IStatusMessage status, IMessageParameters parameters)
		{
			throw new NotImplementedException();
		}

		public void SubscribeToEvents(OnNetworkEventMessage subscriber)
		{
			throw new NotImplementedException();
		}

		public void SubscribeToRequests(OnNetworkRequestMessage subscriber)
		{
			throw new NotImplementedException();
		}

		public void SubscribeToResponses(OnNetworkResponseMessage subscriber)
		{
			throw new NotImplementedException();
		}

		public void SubscribeToStatusChanges(OnNetworkStatusMessage subscriber)
		{
			throw new NotImplementedException();
		}
	}
}
