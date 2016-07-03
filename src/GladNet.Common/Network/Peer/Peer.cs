using Common.Logging;
using Easyception;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public abstract class Peer : INetPeer, IClassLogger, IDisconnectable
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

		public ILog Logger { get; private set; }

		protected IDisconnectionServiceHandler disconnectionHandler { get; private set; }

		protected Peer(ILog logger, INetworkMessageSender messageSender, IConnectionDetails details, INetworkMessageSubscriptionService subService,
			IDisconnectionServiceHandler disconnectHandler)
		{
			Throw<ArgumentNullException>.If.IsNull(logger)?.Now(nameof(logger));
			Throw<ArgumentNullException>.If.IsNull(messageSender)?.Now(nameof(messageSender));
			Throw<ArgumentNullException>.If.IsNull(details)?.Now(nameof(details));
			Throw<ArgumentNullException>.If.IsNull(subService)?.Now(nameof(subService));
			Throw<ArgumentNullException>.If.IsNull(disconnectHandler)?.Now(nameof(disconnectHandler));

			PeerDetails = details;
			NetworkSendService = messageSender;
			Logger = logger;
			disconnectionHandler = disconnectHandler;

			//All peers should care about status changes so we subscribe
			subService.SubscribeTo<StatusMessage>()
				.With(OnReceiveStatus);
		}

		public void Disconnect()
		{
			//Just request a disconnection from the service.
			disconnectionHandler.Disconnect();
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
			Throw<ArgumentNullException>.If.IsNull(message)?.Now(nameof(message));

			//I know I cast here so let's only call this once for efficiency
			NetStatus s = message.Status;

			//Set the new status to the incoming status
			Status = s;

			if (s != Status)
				OnStatusChanged(s);
		}
	}
}
