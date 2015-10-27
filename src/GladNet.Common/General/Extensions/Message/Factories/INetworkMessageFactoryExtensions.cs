using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public static class INetworkMessageFactoryExtensions
	{
		public static StatusMessage WithStatus<TStatusMessageType>(this INetworkMessageFactory<TStatusMessageType> factory, NetStatus status)
			where TStatusMessageType : StatusMessage
		{
			return factory.With(new StatusChangePayload(status));
		}

		public static StatusMessage WithStatus<TStatusMessageType>(this INetworkMessageFactory<TStatusMessageType> factory, StatusChangePayload payload)
			where TStatusMessageType : StatusMessage
		{
			return factory.With(payload);
		}
	}
}
