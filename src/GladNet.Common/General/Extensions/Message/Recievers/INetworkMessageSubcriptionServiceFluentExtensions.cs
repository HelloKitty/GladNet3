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
		public static INetworkMessageSubcriptionFluentBuilder<TNetworkMessageType> SubscribeTo<TNetworkMessageType>(this INetworkMessageSubscriptionService service)
			where TNetworkMessageType : INetworkMessage
		{
			return new NetworkMessageSubscriptionFluentBuilder<TNetworkMessageType>(service);
		}

		/// <summary>
		/// Subscribes the target delegate to <see cref="IEventMessage"/> publisher.
		/// </summary>
		/// <param name="builder">Instance of the fluent builder that carries Type information.</param>
		/// <param name="subscriber">Subscriber delegate object.</param>
		/// <returns>Fluently returns the instance this method was invoked on.</returns>
		public static INetworkMessageSubcriptionFluentBuilder<IEventMessage> With(this INetworkMessageSubcriptionFluentBuilder<IEventMessage> builder, OnNetworkEventMessage subscriber)
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
		public static INetworkMessageSubcriptionFluentBuilder<IResponseMessage> With(this INetworkMessageSubcriptionFluentBuilder<IResponseMessage> builder, OnNetworkResponseMessage subscriber)
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
		public static INetworkMessageSubcriptionFluentBuilder<IRequestMessage> With(this INetworkMessageSubcriptionFluentBuilder<IRequestMessage> builder, OnNetworkRequestMessage subscriber)
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
		public static INetworkMessageSubcriptionFluentBuilder<IStatusMessage> With(this INetworkMessageSubcriptionFluentBuilder<IStatusMessage> builder, OnNetworkStatusMessage subscriber)
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
		public static INetworkMessageSubcriptionFluentBuilder<EventMessage> With(this INetworkMessageSubcriptionFluentBuilder<EventMessage> builder, OnNetworkEventMessage subscriber)
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
		public static INetworkMessageSubcriptionFluentBuilder<ResponseMessage> With(this INetworkMessageSubcriptionFluentBuilder<ResponseMessage> builder, OnNetworkResponseMessage subscriber)
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
		public static INetworkMessageSubcriptionFluentBuilder<RequestMessage> With(this INetworkMessageSubcriptionFluentBuilder<RequestMessage> builder, OnNetworkRequestMessage subscriber)
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
		public static INetworkMessageSubcriptionFluentBuilder<StatusMessage> With(this INetworkMessageSubcriptionFluentBuilder<StatusMessage> builder, OnNetworkStatusMessage subscriber)
		{
			builder.Service.SubscribeToStatusChanges(subscriber);

			return builder;
		}
	}
}
