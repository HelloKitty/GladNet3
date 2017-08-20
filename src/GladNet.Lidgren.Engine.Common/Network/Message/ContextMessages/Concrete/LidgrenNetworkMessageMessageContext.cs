using GladNet.Lidgren.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using GladNet.Message;
using GladNet.Serializer;

namespace GladNet.Lidgren.Engine.Common
{
	/// <summary>
	/// Lidgren message context for <see cref="NetworkMessage"/>s.
	/// </summary>
	public class LidgrenNetworkMessageMessageContext : LidgrenMessageContext
	{
		/// <summary>
		/// The generated message.
		/// </summary>
		public NetworkMessage GeneratedNetworkMessage { get; }

		public LidgrenNetworkMessageMessageContext(NetIncomingMessage incomingMessage, IDeserializerStrategy deserializer)
			: base(incomingMessage)
		{
			if (incomingMessage == null) throw new ArgumentNullException(nameof(incomingMessage));
			if (deserializer == null) throw new ArgumentNullException(nameof(deserializer));

			//The message.LengthBytes - message.PositionInBytes is from GladNet1
			//We need to read the byte[] chunk that is from the current position to the end.
			//For why we do this read this old exerp from GladNet2:
			//"Due to message recycling we cannot trust the internal array of data to be of only the information that should be used for this package.
			//We can trust the indicated size, not the length of .Data, and get a byte[] that represents the sent [Data].
			//However, this will incur a GC penalty which may become an issue; more likely to be an issue on clients."
			byte[] bytes = incomingMessage.ReadBytes(incomingMessage.LengthBytes - incomingMessage.PositionInBytes);

			//Deserializer the network message and the payload.
			GeneratedNetworkMessage = deserializer.Deserialize<NetworkMessage>(bytes);
			GeneratedNetworkMessage?.Payload?.Deserialize(deserializer);
		}

		public override bool TryDispatch(INetworkMessageReceiver reciever)
		{
			if (GeneratedNetworkMessage == null)
				return false;

			GeneratedNetworkMessage.Dispatch(reciever, this); //pass the reciever and this (message parameters).

			return true;
		}
	}
}
