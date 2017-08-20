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
		/// <inheritdoc />
		public byte Channel { get; }

		/// <inheritdoc />
		public DeliveryMethod DeliveryMethod { get; }

		/// <inheritdoc />
		public bool Encrypted { get; }

		public LidgrenMessageDetailsAdapter(NetIncomingMessage message, bool wasEncrypted)
		{
			if (message == null) throw new ArgumentNullException(nameof(message));

			Channel = (byte)message.SequenceChannel;
			DeliveryMethod = message.DeliveryMethod.ToGladNet();

			Encrypted = wasEncrypted;
		}
	}
}
