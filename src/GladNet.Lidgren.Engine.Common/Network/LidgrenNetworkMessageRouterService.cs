using GladNet.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GladNet.Common;
using GladNet.Message;
using GladNet.Payload;
using Lidgren.Network;
using GladNet.Lidgren.Common;

namespace GladNet.Lidgren.Engine.Common
{
	/// <summary>
	/// Abstract contract for message routing/sending service for Lidgren peers/connections.
	/// </summary>
	/// <typeparam name="TLidgrenPeerType">Concrete <see cref="NetPeer"/> type (Generic for strongly typed child access).</typeparam>
	public abstract class LidgrenNetworkMessageRouterService<TLidgrenPeerType> : INetworkMessageRouterService
		where TLidgrenPeerType : NetPeer
	{
		private INetworkMessageFactory networkMessageFactory { get; }

		protected TLidgrenPeerType lidgrenNetworkPeer { get; } //protected so child can handle sending

		public LidgrenNetworkMessageRouterService(INetworkMessageFactory messageFactory, TLidgrenPeerType peerObj)
		{
			if (messageFactory == null)
				throw new ArgumentNullException(nameof(messageFactory), $"Cannot provide a null {nameof(INetworkMessageFactory)} service.");

			if (lidgrenNetworkPeer == null)
				throw new ArgumentNullException(nameof(peerObj), $"Cannot provide a null {nameof(TLidgrenPeerType)} service.");

			lidgrenNetworkPeer = peerObj;
			networkMessageFactory = messageFactory;
		}

		/// <summary>
		/// Indicates if the <see cref="OperationType"/> can be sent.
		/// Inheritors implement and configure sendable types.
		/// </summary>
		/// <param name="opType">Operation type to check.</param>
		/// <returns>True if the type can be sent.</returns>
		public abstract bool CanSend(OperationType opType);

		protected abstract NetSendResult SendMessage(INetworkMessage message, DeliveryMethod deliveryMethod, bool encrypt, byte channel);

		public SendResult TryRouteMessage<TMessageType>(TMessageType message, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0) 
			where TMessageType : INetworkMessage, IRoutableMessage, IOperationTypeMappable
		{
			if (!CanSend(message.OperationTypeMappedValue))
				throw new InvalidOperationException($"Cannot send {message.OperationTypeMappedValue} with the {this.GetType().Name} because the service cannot handle that {nameof(OperationType)}.");
			
			return SendValidMessage(message, deliveryMethod, encrypt, channel);
		}

		public SendResult TrySendMessage(OperationType opType, PacketPayload payload, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			if (!CanSend(opType))
				throw new InvalidOperationException($"Cannot send {opType} with the {this.GetType().Name} because the service cannot handle that {nameof(OperationType)}.");

			INetworkMessage message = networkMessageFactory.Create(opType, payload);

			return SendValidMessage(message, deliveryMethod, encrypt, channel);
		}

		private SendResult SendValidMessage(INetworkMessage message, DeliveryMethod deliveryMethod, bool encrypt, byte channel)
		{
			return SendMessage(message, deliveryMethod, encrypt, channel).ToGladNet();
		}

		//TODO: This should be an extension method in GladNet.
		public SendResult TrySendMessage<TPacketType>(OperationType opType, TPacketType payload) 
			where TPacketType : PacketPayload, IStaticPayloadParameters
		{
			return TrySendMessage(opType, payload, payload.DeliveryMethod, payload.Encrypted, payload.Channel);
		}
	}
}
