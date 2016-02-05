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

		protected Peer(ILogger logger, INetworkMessageSender messageSender, IConnectionDetails details)
		{
			logger.ThrowIfNull(nameof(logger));
			messageSender.ThrowIfNull(nameof(messageSender));
			details.ThrowIfNull(nameof(details));

			PeerDetails = details;
			NetworkSendService = messageSender;
			Logger = logger;
		}

		public virtual bool CanSend(OperationType opType)
		{
			return false;
		}
	}
}
