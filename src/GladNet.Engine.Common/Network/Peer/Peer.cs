using Common.Logging;
using Easyception;
using GladNet.Common;
using GladNet.Message;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace GladNet.Engine.Common
{
	/// <summary>
	/// Base class for all network Peers or Sessions in GladNet.
	/// </summary>
	public abstract class Peer : INetPeer, IClassLogger, IDisconnectable
	{
		/// <summary>
		/// Peer's service for sending network messages.
		/// Use Extension methods or specific types for an neater API.
		/// </summary>
		public INetworkMessageRouterService NetworkSendService { get; private set; }

		/// <summary>
		/// Indicates the Network Status of the current <see cref="INetPeer"/>.
		/// </summary>
		public NetStatus Status { get; protected set; }

		/// <summary>
		/// Provides access to various connection related details for a this given Pee's connection.
		/// </summary>
		public IConnectionDetails PeerDetails { get; private set; }

		/// <summary>
		/// Class logging service.
		/// </summary>
		public ILog Logger { get; private set; }

		/// <summary>
		/// Internally available <see cref="IDisconnectionServiceHandler"/> service that child classes
		/// can access and subscribe to.
		/// </summary>
		protected IDisconnectionServiceHandler disconnectionHandler { get; private set; }

		protected Peer(ILog logger, INetworkMessageRouterService messageSender, IConnectionDetails details, INetworkMessageSubscriptionService subService,
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

		/// <summary>
		/// Disconnects the <see cref="Peer"/> object.
		/// </summary>
		public void Disconnect()
		{
			//Just request a disconnection from the service.
			disconnectionHandler.Disconnect();
		}

		//child should override this
		/// <summary>
		/// Indicates if the <see cref="OperationType"/> can be sent with this peer.
		/// </summary>
		/// <param name="opType"><see cref="OperationType"/> to check.</param>
		/// <returns>True if the peer can see the <paramref name="opType"/>.</returns>
		public virtual bool CanSend(OperationType opType)
		{
			return false;
		}

		/// <summary>
		/// Called when <see cref="NetStatus"/> <see cref="Status"/> changes.
		/// Can be overriden to preform actions in child classes.
		/// </summary>
		/// <param name="status">The new <see cref="NetStatus"/> of the <see cref="INetPeer"/> instance.</param>
		protected virtual void OnStatusChanged(NetStatus status)
		{
			//TODO: Logging if debug

			//TODO: Do internal handling for status change events that are ClientPeerSession specific.
		}

		/// <summary>
		/// Internally managed status receival method.
		/// Setup to be called internally using the <see cref="INetworkMessageSubscriptionService"/>.
		/// </summary>
		/// <param name="message">The new <see cref="IStatusMessage"/> message.</param>
		/// <param name="parameters"><see cref="IMessageParameters"/> the <see cref="IStatusMessage"/> was sent with.</param>
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
