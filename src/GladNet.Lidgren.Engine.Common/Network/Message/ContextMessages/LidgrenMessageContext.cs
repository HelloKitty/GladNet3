using GladNet.Common;
using GladNet.Common.Extensions;
using GladNet.Message;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Lidgren.Engine.Common
{

	/// <summary>
	/// Base context for a Lidgren message context.
	/// </summary>
	public abstract class LidgrenMessageContext : IMessageParameters
	{
		/// <summary>
		/// Indicates the context's message type.
		/// </summary>
		public NetIncomingMessageType MessageType { get; }

		/// <summary>
		/// Indicates the unique indentifier
		/// </summary>
		public int ConnectionId { get; }

		/// <summary>
		/// Delivery method used.
		/// </summary>
		public DeliveryMethod DeliveryMethod { get; }

		/// <summary>
		/// Indicates if the message was encrypted
		/// </summary>
		public bool Encrypted { get; }

		/// <summary>
		/// Indicates the channel used.
		/// </summary>
		public byte Channel { get; }

		public NetIncomingMessage IncomingMessage { get; }

		public LidgrenMessageContext(NetIncomingMessage incomingMessage)
		{
			IncomingMessage = incomingMessage;
			ConnectionId = incomingMessage.SenderConnection.RemoteUniqueIdentifier; //this is a sha hash of some stuff that Lidgren does; don't cast to int.
			DeliveryMethod = incomingMessage.DeliveryMethod.ToGladNet();
			Encrypted = false; //TODO: Handle encryption
			Channel = (byte)incomingMessage.SequenceChannel;
			MessageType = incomingMessage.MessageType;
		}

		public abstract bool TryDispatch(INetworkMessageReceiver reciever);

		public virtual void ClearToMessageParametersOnly()
		{

		}
	}
}
