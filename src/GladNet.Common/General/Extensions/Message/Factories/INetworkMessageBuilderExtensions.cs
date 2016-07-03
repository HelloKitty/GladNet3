using GladNet.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public static class INetworkMessageBuilderExtensions
	{
		public static INetworkMessageFluentBuilder<TNetworkMessageType> PrepareMessage<TNetworkMessageType>(this INetworkMessageFactory messageFactory)
			where TNetworkMessageType : NetworkMessage
		{
			messageFactory.ThrowIfNull(nameof(messageFactory));

			return new NetworkMessageDataContainer<TNetworkMessageType>() { Factory = messageFactory };
		}

		public static NetworkMessageDataContainer<StatusMessage> WithPayload(this NetworkMessageDataContainer<StatusMessage> container, StatusChangePayload payload)
		{
			payload.ThrowIfNull(nameof(payload));

			container.Payload = payload;

			return container;
		}

		public static NetworkMessageDataContainer<EventMessage> WithPayload(this NetworkMessageDataContainer<EventMessage> container, PacketPayload payload)
		{
			payload.ThrowIfNull(nameof(payload));

			container.Payload = payload;

			return container;
		}

		public static NetworkMessageDataContainer<RequestMessage> WithPayload(this NetworkMessageDataContainer<RequestMessage> container, PacketPayload payload)
		{
			payload.ThrowIfNull(nameof(payload));

			container.Payload = payload;

			return container;
		}

		public static NetworkMessageDataContainer<ResponseMessage> WithPayload(this NetworkMessageDataContainer<ResponseMessage> container, PacketPayload payload)
		{
			payload.ThrowIfNull(nameof(payload));

			container.Payload = payload;

			return container;
		}

		public static StatusMessage Build(this NetworkMessageDataContainer<StatusMessage> container)
		{
			container.Payload.ThrowIfNull("Container.Payload");
			container.Factory.ThrowIfNull("Container.Factory");

			return container.Factory.CreateStatusMessage(container.Payload as StatusChangePayload);
		}

		public static EventMessage Build(this NetworkMessageDataContainer<EventMessage> container)
		{
			container.Payload.ThrowIfNull("Container.Payload");
			container.Factory.ThrowIfNull("Container.Factory");

			return container.Factory.CreateEventMessage(container.Payload);
		}

		public static RequestMessage Build(this NetworkMessageDataContainer<RequestMessage> container)
		{
			container.Payload.ThrowIfNull("Container.Payload");
			container.Factory.ThrowIfNull("Container.Factory");

			return container.Factory.CreateRequestMessage(container.Payload);
		}

		public static ResponseMessage Build(this NetworkMessageDataContainer<ResponseMessage> container)
		{
			container.Payload.ThrowIfNull("Container.Payload");
			container.Factory.ThrowIfNull("Container.Factory");

			return container.Factory.CreateResponseMessage(container.Payload);
		}
	}
}
