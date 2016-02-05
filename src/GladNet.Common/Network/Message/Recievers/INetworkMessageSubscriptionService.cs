using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	/// <summary>
	/// Implementer offers subscription services for network messages.
	/// </summary>
	public interface INetworkMessageSubscriptionService
	{
		/// <summary>
		/// Subscribes to <see cref="IRequestMessage"/> network messages.
		/// </summary>
		/// <param name="subscriber">Delegate target subscribing.</param>
		void SubscribeToRequests(OnNetworkRequestMessage subscriber);

		/// <summary>
		/// Subscribes to <see cref="IEventMessage"/> network messages.
		/// </summary>
		/// <param name="subscriber">Delegate target subscribing.</param>
		void SubscribeToEvents(OnNetworkEventMessage subscriber);

		/// <summary>
		/// Subscribes to <see cref="IResponseMessage"/> network messages.
		/// </summary>
		/// <param name="subscriber">Delegate target subscribing.</param>
		void SubscribeToResponses(OnNetworkResponseMessage subscriber);

		/// <summary>
		/// Subscribes to <see cref="IStatusMessage"/> network messages.
		/// </summary>
		/// <param name="subscriber">Delegate target subscribing.</param>
		void SubscribeToStatusChanges(OnNetworkStatusMessage subscriber);
	}
}
