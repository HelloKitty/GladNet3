using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public static class INetworkMessageSubcriptionServiceFluentExtensions
	{
		public static INetworkMessageSubcriptionBuilder<TNetworkMessageType> SubscribeTo<TNetworkMessageType>(this INetworkMessageSubscriptionService service)
			where TNetworkMessageType : INetworkMessage
		{
			return new NetworkMessageSubscriptionBuilder<TNetworkMessageType>(service);
		}

		public static INetworkMessageSubcriptionBuilder<IEventMessage> With(this INetworkMessageSubcriptionBuilder<IEventMessage> builder, OnNetworkEventMessage subscriber)
		{
			builder.Service.SubscribeToEvents(subscriber);

			return builder;
		}

		public static INetworkMessageSubcriptionBuilder<IResponseMessage> With(this INetworkMessageSubcriptionBuilder<IResponseMessage> builder, OnNetworkResponseMessage subscriber)
		{
			builder.Service.SubscribeToResponses(subscriber);

			return builder;
		}

		public static INetworkMessageSubcriptionBuilder<IRequestMessage> With(this INetworkMessageSubcriptionBuilder<IRequestMessage> builder, OnNetworkRequestMessage subscriber)
		{
			builder.Service.SubscribeToRequests(subscriber);

			return builder;
		}

		public static INetworkMessageSubcriptionBuilder<IStatusMessage> With(this INetworkMessageSubcriptionBuilder<IStatusMessage> builder, OnNetworkStatusMessage subscriber)
		{
			builder.Service.SubscribeToStatusChanges(subscriber);

			return builder;
		}

		public static INetworkMessageSubcriptionBuilder<EventMessage> With(this INetworkMessageSubcriptionBuilder<EventMessage> builder, OnNetworkEventMessage subscriber)
		{
			builder.Service.SubscribeToEvents(subscriber);

			return builder;
		}

		public static INetworkMessageSubcriptionBuilder<ResponseMessage> With(this INetworkMessageSubcriptionBuilder<ResponseMessage> builder, OnNetworkResponseMessage subscriber)
		{
			builder.Service.SubscribeToResponses(subscriber);

			return builder;
		}

		public static INetworkMessageSubcriptionBuilder<RequestMessage> With(this INetworkMessageSubcriptionBuilder<RequestMessage> builder, OnNetworkRequestMessage subscriber)
		{
			builder.Service.SubscribeToRequests(subscriber);

			return builder;
		}

		public static INetworkMessageSubcriptionBuilder<StatusMessage> With(this INetworkMessageSubcriptionBuilder<StatusMessage> builder, OnNetworkStatusMessage subscriber)
		{
			builder.Service.SubscribeToStatusChanges(subscriber);

			return builder;
		}
	}
}
