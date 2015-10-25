using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public class GeneralNetworkMessageFactory : INetworkMessageFactory
	{
		private readonly Func<PacketPayload, NetworkMessage> messageProducer;

		public GeneralNetworkMessageFactory(Func<PacketPayload, NetworkMessage> messageFunc)
		{
			if (messageFunc == null)
				throw new ArgumentNullException("messageFunc", "Invalid Func<> or Lambda expression. Resulted in null.");

			messageProducer = messageFunc;
		}

		public NetworkMessage With<TPayload>(TPayload payload) 
			where TPayload : PacketPayload
		{
			return messageProducer.Invoke(payload);
		}

		public NetworkMessage WithDefaultConstructed<TPayload>()
			where TPayload : PacketPayload, new()
		{
			return messageProducer(new TPayload());
		}
	}
}
