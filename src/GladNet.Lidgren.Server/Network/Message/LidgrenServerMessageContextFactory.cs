using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using GladNet.Lidgren.Engine.Common;
using GladNet.Serializer;

namespace GladNet.Lidgren.Server
{
	public class LidgrenServerMessageContextFactory : ILidgrenMessageContextFactory
	{
		/// <summary>
		/// The deserialization service.
		/// </summary>
		private IDeserializerStrategy deserializer { get; }

		public LidgrenServerMessageContextFactory(IDeserializerStrategy deserializationStrategy)
		{
			if (deserializationStrategy == null) throw new ArgumentNullException(nameof(deserializationStrategy));

			deserializer = deserializationStrategy;
		}

		/// <inheritdoc />
		public bool CanCreateContext(NetIncomingMessageType messageType)
		{
			return messageType == NetIncomingMessageType.StatusChanged || messageType == NetIncomingMessageType.Data;
		}

		/// <inheritdoc />
		public LidgrenMessageContext CreateContext(NetIncomingMessage message)
		{
			switch (message.MessageType)
			{
				//TODO: Do error messages
				//case NetIncomingMessageType.Error:
				//	break;
				case NetIncomingMessageType.ErrorMessage:
				case NetIncomingMessageType.Error:
					throw new Exception("Error recieved.");
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
