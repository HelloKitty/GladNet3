using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public class NetworkMessagePublisher : INetworkMessageReceiver, INetworkMessageSubscriptionService, INetworkMessagePublisher
	{
		//You may wonder why there is no locking or effort to keep this thread safe.
		//This is a critical section of code that should be preformant and there isn't a benefit to locking
		//Why are multiple threads subscribing and unsubing from the publisher? There really is no use-case
		//Therefore although it'd be trivial to do I will not make this thread safe.

		/// <summary>
		/// Event channel.
		/// </summary>
		public event OnNetworkEventMessage EventPublisher;

		/// <summary>
		/// Request channel.
		/// </summary>
		public event OnNetworkRequestMessage RequestPublisher;

		/// <summary>
		/// Response channel.
		/// </summary>
		public event OnNetworkResponseMessage ResponsePublisher;

		/// <summary>
		/// Status channel.
		/// </summary>
		public event OnNetworkStatusMessage StatusPublisher;

		public void OnNetworkMessageReceive(IEventMessage message, IMessageParameters parameters)
		{
			message.ThrowIfNull(nameof(message));

			if (EventPublisher != null)
				EventPublisher.Invoke(message, parameters);
		}

		public void OnNetworkMessageReceive(IResponseMessage message, IMessageParameters parameters)
		{
			message.ThrowIfNull(nameof(message));

			if (ResponsePublisher != null)
				ResponsePublisher.Invoke(message, parameters);
		}

		public void OnNetworkMessageReceive(IRequestMessage message, IMessageParameters parameters)
		{
			message.ThrowIfNull(nameof(message));

			if (RequestPublisher != null)
				RequestPublisher.Invoke(message, parameters);
		}

		public void OnNetworkMessageReceive(IStatusMessage status, IMessageParameters parameters)
		{
			status.ThrowIfNull(nameof(status));

			if (StatusPublisher != null)
				StatusPublisher.Invoke(status, parameters);
		}

		public void SubscribeToEvents(OnNetworkEventMessage subscriber)
		{
			subscriber.ThrowIfNull(nameof(subscriber));

			EventPublisher += subscriber;
		}

		public void SubscribeToRequests(OnNetworkRequestMessage subscriber)
		{
			subscriber.ThrowIfNull(nameof(subscriber));

			RequestPublisher += subscriber;
		}

		public void SubscribeToResponses(OnNetworkResponseMessage subscriber)
		{
			subscriber.ThrowIfNull(nameof(subscriber));

			ResponsePublisher += subscriber;
		}

		public void SubscribeToStatusChanges(OnNetworkStatusMessage subscriber)
		{
			subscriber.ThrowIfNull(nameof(subscriber));

			StatusPublisher += subscriber;
		}
	}
}
