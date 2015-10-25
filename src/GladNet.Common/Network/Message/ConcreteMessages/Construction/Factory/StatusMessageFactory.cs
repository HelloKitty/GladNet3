using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public class StatusMessageFactory : INetworkMessageFactory
	{
		public NetworkMessage With<TPayload>(TPayload payload)
			where TPayload : PacketPayload
		{
			if (payload == null)
				throw new ArgumentNullException("payload", "Payload cannot be null when creating a NetworkMessage.");

			try
			{
				return new StatusMessage(payload as StatusChangePayload);
			}
			catch(ArgumentNullException e)
			{
				throw new ArgumentException("Likely payload was not of proper Type. Expecting StatusChangePayload.", e);
			}
		}

		NetworkMessage INetworkMessageFactory.WithDefaultConstructed<TPayload>()
		{
			if (typeof(TPayload) != typeof(StatusChangePayload))
				throw new ArgumentException("Generic Type of PacketPayload is invalid. Expected: " + typeof(StatusChangePayload) + " but got " + typeof(TPayload), "TPayload");

			return With(new TPayload());
		}
	}
}
