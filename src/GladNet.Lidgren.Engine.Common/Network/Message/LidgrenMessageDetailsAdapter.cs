using GladNet.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GladNet.Common;
using Lidgren.Network;
using GladNet.Common.Extensions;

namespace GladNet.Lidgren.Engine.Common
{
	public class LidgrenMessageDetailsAdapter : IMessageParameters
	{
		public byte Channel { get; }

		public DeliveryMethod DeliveryMethod { get; }

		public bool Encrypted { get; }

		public LidgrenMessageDetailsAdapter(NetIncomingMessage message, bool wasEncrypted)
		{
			Channel = (byte)message.SequenceChannel;
			DeliveryMethod = message.DeliveryMethod.ToGladNet();

			Encrypted = wasEncrypted;
		}
	}
}
