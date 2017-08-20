using GladNet.Lidgren.Engine.Common;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GladNet.Common;
using GladNet.Message;
using GladNet.Payload;
using GladNet.Serializer;
using GladNet.Common.Extensions;

namespace GladNet.Lidgren.Client
{
	public class LidgrenClientNetworkMessageRouterService : LidgrenNetworkMessageRouterService
	{
		/// <summary>
		/// Serialization strategy.
		/// </summary>
		private ISerializerStrategy serializer { get; }

		/// <summary>
		/// Creates a new <see cref="NetClient"/>-based network message router service for Lidgren.
		/// </summary>
		/// <param name="messageFactory">The network message factory.</param>
		/// <param name="peerObj">The <see cref="NetPeer"/> object.</param>
		/// <param name="serializerStrategy">Serialization strategy.</param>
		public LidgrenClientNetworkMessageRouterService(INetworkMessageFactory messageFactory, NetConnection connection, ISerializerStrategy serializerStrategy)
			: base(messageFactory, connection)
		{
			if (serializerStrategy == null)
				throw new ArgumentNullException(nameof(serializerStrategy), $"Cannot provide a null {nameof(ISerializerStrategy)} to {nameof(LidgrenClientNetworkMessageRouterService)}.");

			serializer = serializerStrategy;
		}

		public override bool CanSend(OperationType opType)
		{
			//Clients can only send requests.
			return opType == OperationType.Request;
		}

		/// <summary>
		/// Child implemented sending functionality. Sends a provided <see cref="INetworkMessage"/> strategy
		/// </summary>
		/// <param name="message">Network message to send.</param>
		/// <param name="deliveryMethod">Delivery method to send.</param>
		/// <param name="encrypt">Indicates if the message should be encrypted before being sent.</param>
		/// <param name="channel"></param>
		/// <returns></returns>
		protected override NetSendResult SendMessage(INetworkMessage message, DeliveryMethod deliveryMethod, bool encrypt, byte channel)
		{
			if (this.lidgrenNetworkConnection.Peer.Status != NetPeerStatus.Running)
#if DEBUG || DEBUGBUILD
				throw new InvalidOperationException($"The {this.lidgrenNetworkConnection.GetType().Name} is not currently running.");
#else
				return NetSendResult.FailedNotConnected;
#endif
			NetOutgoingMessage outgoingMessage = this.lidgrenNetworkConnection.Peer.CreateMessage(); //TODO: Create a system to estimate message size.

			//We only need to serialize and send for user-messages
			outgoingMessage.Write(message.SerializeWithVisitor(serializer));

			return this.lidgrenNetworkConnection.SendMessage(outgoingMessage, deliveryMethod.ToLidgren(), channel);
		}
	}
}
