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
		private readonly INetworkMessageSender netMessageSender;

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
			netMessageSender = messageSender;
			Logger = logger;
		}

		#region Message Senders
		//The idea here is we return invalids because sometimes a Peer can't send a certain message type.
		//In most cases external classes shouldn't be interfacing with this class in this fashion.
		//They should instead used more explict send methods. However, this exists to allow for
		//users to loosely couple their senders as best they can though they really shouldn't since
		//it can't be known if the runetime Peer type offers that functionality.
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public virtual SendResult TrySendMessage(OperationType opType, PacketPayload payload, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			payload.ThrowIfNull(nameof(payload));

			//TODO: Implement logging.
			if (!CanSend(opType))
				return SendResult.Invalid;

			return netMessageSender.TrySendMessage(opType, payload, deliveryMethod, encrypt, channel); //ncrunch: no coverage Reason: The line doesn't have to be tested. This is abstract and can be overidden.
		}

		//This is non-virtual because it should mirror non-generic methods functionality. It makes no sense to change them individually.
		public SendResult TrySendMessage<TPacketType>(OperationType opType, TPacketType payload) where TPacketType : PacketPayload, IStaticPayloadParameters
		{
			payload.ThrowIfNull(nameof(payload));

			return TrySendMessage(opType, payload, payload.DeliveryMethod, payload.Encrypted, payload.Channel);
		}
		#endregion

		protected abstract void OnStatusChanged(NetStatus status);

		public virtual bool CanSend(OperationType opType)
		{
			return false;
		}
	}
}
