using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public class DispatchMessage : IMessageParameters
	{
		private readonly NetworkMessage message;

		public NetworkMessage Message
		{
			get { return message; }
		}

		public DeliveryMethod DeliveryMethod { get; private set; }

		/// <summary>
		/// True indicates is/was encrypted on the wire.
		/// </summary>
		public bool Encrypted { get; private set; }

		public byte Channel { get; private set; }

		public DispatchMessage(NetworkMessage mess, DeliveryMethod deliveryMethod, bool encrypt, byte channel)
		{
			message = mess;
			DeliveryMethod = deliveryMethod;
			Encrypted = encrypt;
			Channel = channel;
		}
	}
}
