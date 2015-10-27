using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public class StatusMessageFactory : GeneralNetworkMessageFactory<StatusMessage>, INetworkMessageFactory<StatusMessage>
	{
		public StatusMessageFactory()
			: base(p => new StatusMessage(p as StatusChangePayload))
		{

		}

		public override StatusMessage With<TPayload>(TPayload payload)
		{
			if (payload == null)
				throw new ArgumentNullException("payload", "Payload cannot be null when creating a NetworkMessage.");

			try
			{
				return base.With(payload);
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

			return base.WithDefaultConstructed<TPayload>();
		}
	}
}
