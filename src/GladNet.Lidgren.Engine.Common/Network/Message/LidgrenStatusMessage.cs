using GladNet.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GladNet.Common;
using GladNet.Payload;
using GladNet.Serializer;
using Lidgren.Network;
using GladNet.Lidgren.Common;

namespace GladNet.Lidgren.Engine.Common
{
	public class LidgrenStatusMessage : IStatusMessage
	{
		public NetSendable<PacketPayload> Payload
		{
			get
			{
				throw new NotImplementedException($"Lidgren has not generated a {nameof(NetSendable<PacketPayload>)} for the fake status message.");
			}
		}

		public NetStatus Status { get; }

		public LidgrenStatusMessage(NetConnectionStatus status)
		{
			Status = status.ToGladNet();
		}

		public byte[] SerializeWithVisitor(ISerializerStrategy serializer)
		{
			//We can only serializer this type by pretending it's a real StatusMessage.
			//Probably what everyone expects anyway.
			return serializer.Serialize(new StatusMessage(new StatusChangePayload(Status)));
		}
	}
}
