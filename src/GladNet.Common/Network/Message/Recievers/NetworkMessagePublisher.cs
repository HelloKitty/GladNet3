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

		/// <summary>
		/// Interface method overload for receiving a <see cref="IEventMessage"/>.
		/// </summary>
		/// <param name="message">The event recieved from the remote peer.</param>
		/// <param name="parameters">The message parameters the message was sent with.</param>
		public void OnNetworkMessageReceive(IEventMessage message, IMessageParameters parameters)
		{
			message.ThrowIfNull(nameof(message));

			if (EventPublisher != null)
				EventPublisher.Invoke(message, parameters);
		}

		/// <summary>
		/// Interface method overload for receiving a <see cref="IResponseMessage"/>.
		/// </summary>
		/// <param name="message">The response recieved from the remote peer.</param>
		/// <param name="parameters">The message parameters the message was sent with.</param>
		public void OnNetworkMessageReceive(IResponseMessage message, IMessageParameters parameters)
		{
			message.ThrowIfNull(nameof(message));

			if (ResponsePublisher != null)
				ResponsePublisher.Invoke(message, parameters);
		}

		/// <summary>
		/// Interface method overload for receiving a <see cref="IRequestMessage"/>.
		/// </summary>
		/// <param name="message">The request recieved from the remote peer.</param>
		/// <param name="parameters">The message parameters the message was sent with.</param>
		public void OnNetworkMessageReceive(IRequestMessage message, IMessageParameters parameters)
		{
			message.ThrowIfNull(nameof(message));

			if (RequestPublisher != null)
				RequestPublisher.Invoke(message, parameters);
		}

		/// <summary>
		/// Dispatchable method that handles <see cref="IStatusMessage"/> changes.
		/// </summary>
		/// <param name="status">The status message recieved from the remote peer.</param>
		/// <param name="parameters">The message parameters the message was sent with.</param>
		public void OnNetworkMessageReceive(IStatusMessage status, IMessageParameters parameters)
		{
			status.ThrowIfNull(nameof(status));

			if (StatusPublisher != null)
				StatusPublisher.Invoke(status, parameters);
		}

		/// <summary>
		/// Subscribes to <see cref="IEventMessage"/> network messages.
		/// </summary>
		/// <param name="subscriber">Delegate target subscribing.</param>
		public void SubscribeToEvents(OnNetworkEventMessage subscriber)
		{
			subscriber.ThrowIfNull(nameof(subscriber));

			EventPublisher += subscriber;
		}

		/// <summary>
		/// Subscribes to <see cref="IRequestMessage"/> network messages.
		/// </summary>
		/// <param name="subscriber">Delegate target subscribing.</param>
		public void SubscribeToRequests(OnNetworkRequestMessage subscriber)
		{
			subscriber.ThrowIfNull(nameof(subscriber));

			RequestPublisher += subscriber;
		}

		/// <summary>
		/// Subscribes to <see cref="IResponseMessage"/> network messages.
		/// </summary>
		/// <param name="subscriber">Delegate target subscribing.</param>
		public void SubscribeToResponses(OnNetworkResponseMessage subscriber)
		{
			subscriber.ThrowIfNull(nameof(subscriber));

			ResponsePublisher += subscriber;
		}

		/// <summary>
		/// Subscribes to <see cref="IStatusMessage"/> network messages.
		/// </summary>
		/// <param name="subscriber">Delegate target subscribing.</param>
		public void SubscribeToStatusChanges(OnNetworkStatusMessage subscriber)
		{
			subscriber.ThrowIfNull(nameof(subscriber));

			StatusPublisher += subscriber;
		}
	}
}
