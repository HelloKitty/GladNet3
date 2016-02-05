using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public static class INetworkMessageSubcriptionServiceFluentExtensions
	{
		/// <summary>
		/// Creates a builder object that can service subscription requests that does not require explict building.
		/// </summary>
		/// <typeparam name="TNetworkMessageType">Type of the network mesage to subscribe to.</typeparam>
		/// <param name="service">Subscription service instance.</param>
		/// <returns>A subscription builder that carries Type information that allows for subscriptions.</returns>
		public static INetworkMessageSubcriptionBuilder<TNetworkMessageType> SubscribeTo<TNetworkMessageType>(this INetworkMessageSubscriptionService service)
			where TNetworkMessageType : INetworkMessage
		{
			return new NetworkMessageSubscriptionBuilder<TNetworkMessageType>(service);
		}

		/// <summary>
		/// Subscribes the target delegate to <see cref="IEventMessage"/> publisher.
		/// </summary>
		/// <param name="builder">Instance of the fluent builder that carries Type information.</param>
		/// <param name="subscriber">Subscriber delegate object.</param>
		/// <returns>Fluently returns the instance this method was invoked on.</returns>
		public static INetworkMessageSubcriptionBuilder<IEventMessage> With(this INetworkMessageSubcriptionBuilder<IEventMessage> builder, OnNetworkEventMessage subscriber)
		{
			builder.Service.SubscribeToEvents(subscriber);

			return builder;
		}

		/// <summary>
		/// Subscribes the target delegate to <see cref="IResponseMessage"/> publisher.
		/// </summary>
		/// <param name="builder">Instance of the fluent builder that carries Type information.</param>
		/// <param name="subscriber">Subscriber delegate object.</param>
		/// <returns>Fluently returns the instance this method was invoked on.</returns>
		public static INetworkMessageSubcriptionBuilder<IResponseMessage> With(this INetworkMessageSubcriptionBuilder<IResponseMessage> builder, OnNetworkResponseMessage subscriber)
		{
			builder.Service.SubscribeToResponses(subscriber);

			return builder;
		}

		/// <summary>
		/// Subscribes the target delegate to <see cref="IRequestMessage"/> publisher.
		/// </summary>
		/// <param name="builder">Instance of the fluent builder that carries Type information.</param>
		/// <param name="subscriber">Subscriber delegate object.</param>
		/// <returns>Fluently returns the instance this method was invoked on.</returns>
		public static INetworkMessageSubcriptionBuilder<IRequestMessage> With(this INetworkMessageSubcriptionBuilder<IRequestMessage> builder, OnNetworkRequestMessage subscriber)
		{
			builder.Service.SubscribeToRequests(subscriber);

			return builder;
		}

		/// <summary>
		/// Subscribes the target delegate to <see cref="IStatusMessage"/> publisher.
		/// </summary>
		/// <param name="builder">Instance of the fluent builder that carries Type information.</param>
		/// <param name="subscriber">Subscriber delegate object.</param>
		/// <returns>Fluently returns the instance this method was invoked on.</returns>
		public static INetworkMessageSubcriptionBuilder<IStatusMessage> With(this INetworkMessageSubcriptionBuilder<IStatusMessage> builder, OnNetworkStatusMessage subscriber)
		{
			builder.Service.SubscribeToStatusChanges(subscriber);

			return builder;
		}

		/// <summary>
		/// Subscribes the target delegate to <see cref="EventMessage"/> publisher.
		/// </summary>
		/// <param name="builder">Instance of the fluent builder that carries Type information.</param>
		/// <param name="subscriber">Subscriber delegate object.</param>
		/// <returns>Fluently returns the instance this method was invoked on.</returns>
		public static INetworkMessageSubcriptionBuilder<EventMessage> With(this INetworkMessageSubcriptionBuilder<EventMessage> builder, OnNetworkEventMessage subscriber)
		{
			builder.Service.SubscribeToEvents(subscriber);

			return builder;
		}

		/// <summary>
		/// Subscribes the target delegate to <see cref="ResponseMessage"/> publisher.
		/// </summary>
		/// <param name="builder">Instance of the fluent builder that carries Type information.</param>
		/// <param name="subscriber">Subscriber delegate object.</param>
		/// <returns>Fluently returns the instance this method was invoked on.</returns>
		public static INetworkMessageSubcriptionBuilder<ResponseMessage> With(this INetworkMessageSubcriptionBuilder<ResponseMessage> builder, OnNetworkResponseMessage subscriber)
		{
			builder.Service.SubscribeToResponses(subscriber);

			return builder;
		}

		/// <summary>
		/// Subscribes the target delegate to <see cref="RequestMessage"/> publisher.
		/// </summary>
		/// <param name="builder">Instance of the fluent builder that carries Type information.</param>
		/// <param name="subscriber">Subscriber delegate object.</param>
		/// <returns>Fluently returns the instance this method was invoked on.</returns>
		public static INetworkMessageSubcriptionBuilder<RequestMessage> With(this INetworkMessageSubcriptionBuilder<RequestMessage> builder, OnNetworkRequestMessage subscriber)
		{
			builder.Service.SubscribeToRequests(subscriber);

			return builder;
		}

		/// <summary>
		/// Subscribes the target delegate to <see cref="StatusMessage"/> publisher.
		/// </summary>
		/// <param name="builder">Instance of the fluent builder that carries Type information.</param>
		/// <param name="subscriber">Subscriber delegate object.</param>
		/// <returns>Fluently returns the instance this method was invoked on.</returns>
		public static INetworkMessageSubcriptionBuilder<StatusMessage> With(this INetworkMessageSubcriptionBuilder<StatusMessage> builder, OnNetworkStatusMessage subscriber)
		{
			builder.Service.SubscribeToStatusChanges(subscriber);

			return builder;
		}
	}
}
