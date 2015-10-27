using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public static class GeneralNetworkMessageFactory
	{
		public static GeneralNetworkMessageFactory<TNetworkMessageType> Create<TNetworkMessageType>(Func<PacketPayload, TNetworkMessageType> messageFunc)
			where TNetworkMessageType : NetworkMessage
		{
			return new GeneralNetworkMessageFactory<TNetworkMessageType>(messageFunc);
		}
	}

	public class GeneralNetworkMessageFactory<TNetworkMessageType> : INetworkMessageFactory<TNetworkMessageType>
		where TNetworkMessageType : NetworkMessage
	{
		private readonly Func<PacketPayload, TNetworkMessageType> messageProducer;

		public GeneralNetworkMessageFactory(Func<PacketPayload, TNetworkMessageType> messageFunc)
		{
			if (messageFunc == null)
				throw new ArgumentNullException("messageFunc", "Invalid Func<> or Lambda expression. Resulted in null.");

			messageProducer = messageFunc;
		}

		public virtual TNetworkMessageType With<TPayload>(TPayload payload) 
			where TPayload : PacketPayload
		{
			return messageProducer.Invoke(payload);
		}

		public virtual TNetworkMessageType WithDefaultConstructed<TPayload>()
			where TPayload : PacketPayload, new()
		{
			return messageProducer(new TPayload());
		}

		NetworkMessage INetworkMessageFactory.With<TPayload>(TPayload payload)
		{
			return ((INetworkMessageFactory<TNetworkMessageType>)this).With(payload);
		}

		NetworkMessage INetworkMessageFactory.WithDefaultConstructed<TPayload>()
		{
			return ((INetworkMessageFactory<TNetworkMessageType>)this).WithDefaultConstructed<TPayload>();
		}
	}
}
