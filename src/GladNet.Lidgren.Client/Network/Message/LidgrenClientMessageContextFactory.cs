using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using GladNet.Lidgren.Engine.Common;
using GladNet.Serializer;

namespace GladNet.Lidgren.Client
{
	public class LidgrenClientMessageContextFactory : ILidgrenMessageContextFactory
	{
		private IDeserializerStrategy deserializer { get; }

		public LidgrenClientMessageContextFactory(IDeserializerStrategy deserializationStrategy)
		{
			if (deserializationStrategy == null)
				throw new ArgumentNullException(nameof(deserializationStrategy), $"Provided {nameof(IDeserializerStrategy)} cannot be null.");

			deserializer = deserializationStrategy;
		}

		public bool CanCreateContext(NetIncomingMessageType messageType)
		{
			return messageType == NetIncomingMessageType.StatusChanged || messageType == NetIncomingMessageType.Data;
		}

		public LidgrenMessageContext CreateContext(NetIncomingMessage message)
		{
			switch (message.MessageType)
			{
				//TODO: Do error messages
				//case NetIncomingMessageType.Error:
				//	break;
				case NetIncomingMessageType.StatusChanged:
					return new LidgrenStatusChangeMessageContext(message); //returns a new status message context
				case NetIncomingMessageType.Data:
					return new LidgrenNetworkMessageMessageContext(message, deserializer);
				default:
					throw new InvalidOperationException($"Failed to generate Context for {message.MessageType}.");
			}
		}
	}
}
