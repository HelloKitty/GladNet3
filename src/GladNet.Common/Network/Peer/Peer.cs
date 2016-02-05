using Logging.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public abstract class Peer : INetPeer, IClassLogger
	{
		/// <summary>
		/// Peer's service for sending network messages.
		/// Use Extension methods or specific types for an neater API.
		/// </summary>
		public INetworkMessageSender NetworkSendService { get; private set; }

		public NetStatus Status { get; protected set; }

		/// <summary>
		/// Provides access to various connection related details for a this given Pee's connection.
		/// </summary>
		public IConnectionDetails PeerDetails { get; private set; }

		public ILogger Logger { get; private set; }

		protected Peer(ILogger logger, INetworkMessageSender messageSender, IConnectionDetails details, INetworkMessageSubscriptionService subService)
		{
			logger.ThrowIfNull(nameof(logger));
			messageSender.ThrowIfNull(nameof(messageSender));
			details.ThrowIfNull(nameof(details));
			subService.ThrowIfNull(nameof(subService));

			PeerDetails = details;
			NetworkSendService = messageSender;
			Logger = logger;

			//All peers should care about status changes so we subscribe
			subService.SubscribeTo<StatusMessage>()
				.With(OnReceiveStatus);
		}

		public virtual bool CanSend(OperationType opType)
		{
			return false;
		}

		protected virtual void OnStatusChanged(NetStatus status)
		{
			//TODO: Logging if debug

			//TODO: Do internal handling for status change events that are ClientPeerSession specific.
		}

		private void OnReceiveStatus(IStatusMessage message, IMessageParameters parameters)
		{
			message.ThrowIfNull(nameof(message));

			//I know I cast here so let's only call this once for efficiency
			NetStatus s = message.Status;
			if (s != Status)
				OnStatusChanged(s);
		}
	}
}
