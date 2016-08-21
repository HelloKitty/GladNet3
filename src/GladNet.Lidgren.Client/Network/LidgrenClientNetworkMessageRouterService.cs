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
	public class LidgrenClientNetworkMessageRouterService : LidgrenNetworkMessageRouterService<NetClient>
	{
		private ISerializerStrategy serializer { get; }

		public LidgrenClientNetworkMessageRouterService(INetworkMessageFactory messageFactory, NetClient peerObj, ISerializerStrategy serializerStrategy)
			: base(messageFactory, peerObj)
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

		protected override NetSendResult SendMessage(INetworkMessage message, DeliveryMethod deliveryMethod, bool encrypt, byte channel)
		{
			if (this.lidgrenNetworkPeer.Status != NetPeerStatus.Running)
#if DEBUG || DEBUGBUILD
				throw new InvalidOperationException($"The {this.lidgrenNetworkPeer.GetType().Name} is not currently running.");
#else
				return NetSendResult.FailedNotConnected;
#endif

			NetOutgoingMessage outgoingMessage = this.lidgrenNetworkPeer.CreateMessage(); //TODO: Create a system to estimate message size.

			//We only need to serialize and send for user-messages
			outgoingMessage.Write(message.SerializeWithVisitor(serializer));

			return this.lidgrenNetworkPeer.SendMessage(outgoingMessage, deliveryMethod.ToLidgren());
		}
	}
}
