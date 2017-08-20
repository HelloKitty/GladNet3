using GladNet.Lidgren.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using GladNet.Common;
using GladNet.Common.Extensions;
using GladNet.Engine.Common;
using GladNet.Message;
using GladNet.Serializer;
using INetworkMessageFactory = GladNet.Lidgren.Engine.Common.INetworkMessageFactory;

namespace GladNet.Lidgren.Server
{
	public class LidgrenServerNetworkMessageRouterService : LidgrenNetworkMessageRouterService
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
		public LidgrenServerNetworkMessageRouterService(INetworkMessageFactory messageFactory, NetConnection connection, ISerializerStrategy serializerStrategy)
			: base(messageFactory, connection)
		{
			if (serializerStrategy == null) throw new ArgumentNullException(nameof(serializerStrategy));

			serializer = serializerStrategy;
		}

		public override bool CanSend(OperationType opType)
		{
			//Can only send events or responses on a server.
			return opType == OperationType.Event || opType == OperationType.Response;
		}

		protected override NetSendResult SendMessage(INetworkMessage message, DeliveryMethod deliveryMethod, bool encrypt, byte channel)
		{
			if (message == null) throw new ArgumentNullException(nameof(message));

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
