using GladNet.Message;
using GladNet.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Engine.Common
{
	public static class INetworkMessageBuilderExtensions
	{
		public static INetworkMessageFluentBuilder<TNetworkMessageType> PrepareMessage<TNetworkMessageType>(this INetworkMessageFactory messageFactory)
			where TNetworkMessageType : NetworkMessage
		{
			if (messageFactory == null) throw new ArgumentNullException(nameof(messageFactory));

			return new NetworkMessageDataContainer<TNetworkMessageType>() { Factory = messageFactory };
		}

		public static NetworkMessageDataContainer<StatusMessage> WithPayload(this NetworkMessageDataContainer<StatusMessage> container, StatusChangePayload payload)
		{
			if (payload == null) throw new ArgumentNullException(nameof(payload));

			container.Payload = payload;

			return container;
		}

		public static NetworkMessageDataContainer<EventMessage> WithPayload(this NetworkMessageDataContainer<EventMessage> container, PacketPayload payload)
		{
			if (payload == null) throw new ArgumentNullException(nameof(payload));

			container.Payload = payload;

			return container;
		}

		public static NetworkMessageDataContainer<RequestMessage> WithPayload(this NetworkMessageDataContainer<RequestMessage> container, PacketPayload payload)
		{
			if (payload == null) throw new ArgumentNullException(nameof(payload));

			container.Payload = payload;

			return container;
		}

		public static NetworkMessageDataContainer<ResponseMessage> WithPayload(this NetworkMessageDataContainer<ResponseMessage> container, PacketPayload payload)
		{
			if (payload == null) throw new ArgumentNullException(nameof(payload));

			container.Payload = payload;

			return container;
		}

		public static StatusMessage Build(this NetworkMessageDataContainer<StatusMessage> container)
		{
			if (container == null) throw new ArgumentNullException(nameof(container));
			if (container.Payload == null) throw new ArgumentNullException(nameof(container.Payload));
			if (container.Payload == null) throw new ArgumentNullException(nameof(container.Factory));

			return container.Factory.CreateStatusMessage(container.Payload as StatusChangePayload);
		}

		public static EventMessage Build(this NetworkMessageDataContainer<EventMessage> container)
		{
			if (container == null) throw new ArgumentNullException(nameof(container));
			if (container.Payload == null) throw new ArgumentNullException(nameof(container.Payload));
			if (container.Payload == null) throw new ArgumentNullException(nameof(container.Factory));

			return container.Factory.CreateEventMessage(container.Payload);
		}

		public static RequestMessage Build(this NetworkMessageDataContainer<RequestMessage> container)
		{
			if (container == null) throw new ArgumentNullException(nameof(container));
			if (container.Payload == null) throw new ArgumentNullException(nameof(container.Payload));
			if (container.Payload == null) throw new ArgumentNullException(nameof(container.Factory));

			return container.Factory.CreateRequestMessage(container.Payload);
		}

		public static ResponseMessage Build(this NetworkMessageDataContainer<ResponseMessage> container)
		{
			if (container == null) throw new ArgumentNullException(nameof(container));
			if (container.Payload == null) throw new ArgumentNullException(nameof(container.Payload));
			if (container.Payload == null) throw new ArgumentNullException(nameof(container.Factory));

			return container.Factory.CreateResponseMessage(container.Payload);
		}
	}
}
