using GladNet.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using GladNet.Serializer;
using GladNet.Message;

namespace GladNet.Lidgren.Engine.Common
{
	public class LidgrenNetworkMessagePublisherService : NetworkMessagePublisher, INetworkMessageDispatcher
	{
		private IDeserializerStrategy deserializer { get; }

		public LidgrenNetworkMessagePublisherService(IDeserializerStrategy deserializerStrat)
		{
			if (deserializerStrat == null)
				throw new ArgumentNullException(nameof(deserializerStrat), $"Must provide non-null {nameof(IDeserializerStrategy)} for {nameof(LidgrenNetworkMessagePublisherService)}");

			deserializer = deserializerStrat;
		}

		public void Dispatch(NetIncomingMessage message)
		{
			//The message.LengthBytes - message.PositionInBytes is from GladNet1
			//We need to read the byte[] chunk that is from the current position to the end.
			//For why we do this read this old exerp from GladNet2:
			//"Due to message recycling we cannot trust the internal array of data to be of only the information that should be used for this package.
			//We can trust the indicated size, not the length of .Data, and get a byte[] that represents the sent [Data].
			//However, this will incur a GC penalty which may become an issue; more likely to be an issue on clients."
			byte[] bytes = message.ReadBytes(message.LengthBytes - message.PositionInBytes);

			NetworkMessage gladNetNetworkMessage = null;
					
			try
			{
				gladNetNetworkMessage = deserializer.Deserialize<NetworkMessage>(bytes);
			}
			catch (Exception e)
			{
				//This is only for debug builds because in release end-user exploiters might send garbage and generate many exceptions
#if DEBUG || DEBUGBUILD
				throw new InvalidOperationException($"Could not deserialize message from ID: {message.SenderConnection?.RemoteUniqueIdentifier}");
#else
				//supress exception
				//Do NOT disconnect the peer due to malformed messages
				//Malicious users will spoof the messages of others in that case
#endif
			}

			gladNetNetworkMessage?.Dispatch(this, new LidgrenMessageDetailsAdapter(message, false)); //TODO: Encryption implementation
		}

		public void Dispatch(NetConnectionStatus status)
		{
			LidgrenStatusMessage message = new LidgrenStatusMessage(status);

			//Just dispatch as a real status message
			this.OnNetworkMessageReceive(message, null);
		}

		//TODO: Add internal message dispatching
	}
}
