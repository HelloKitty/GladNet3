using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common.Tests
{
	[TestFixture(TestOf = typeof(NetworkMessagePublisher))]
	public static class NetworkMessagePublisherTests
	{
		[Test]
		public static void Test_Throws_Exception_On_Null_Subscribe()
		{
			//arrange
			NetworkMessagePublisher publisher = new NetworkMessagePublisher();

			//Assert: All should throw on null.
			Assert.Throws<ArgumentNullException>(() => publisher.SubscribeToEvents(null));
			Assert.Throws<ArgumentNullException>(() => publisher.SubscribeToResponses(null));
			Assert.Throws<ArgumentNullException>(() => publisher.SubscribeToRequests(null));
			Assert.Throws<ArgumentNullException>(() => publisher.SubscribeToStatusChanges(null));
		}

		[Test]
		public static void Test_Doesnt_Throw_On_Valid_Subscription()
		{
			//arrange
			NetworkMessagePublisher publisher = new NetworkMessagePublisher();

			//Assert: All should throw on null.
			Assert.DoesNotThrow(() => publisher.SubscribeToEvents((x, y) => { }));
			Assert.DoesNotThrow(() => publisher.SubscribeToResponses((x, y) => { }));
			Assert.DoesNotThrow(() => publisher.SubscribeToRequests((x, y) => { }));
			Assert.DoesNotThrow(() => publisher.SubscribeToStatusChanges((x, y) => { }));
		}

		[Test]
		public static void Test_Subscribed_Target_Is_Invoked_On_Publish_Event()
		{
			GenericPublishMessageTest<IEventMessage>();
		}

		[Test]
		public static void Test_Subscribed_Target_Is_Invoked_On_Publish_Response()
		{
			GenericPublishMessageTest<IResponseMessage>();
		}

		[Test]
		public static void Test_Subscribed_Target_Is_Invoked_On_Publish_Request()
		{
			GenericPublishMessageTest<IRequestMessage>();
		}

		[Test]
		public static void Test_Subscribed_Target_Is_Invoked_On_Publish_Status()
		{
			GenericPublishMessageTest<IStatusMessage>();
		}

		public static void GenericPublishMessageTest<TNetworkMessageType>()
			where TNetworkMessageType : class, INetworkMessage
		{
			//arrange
			NetworkMessagePublisher publisher = new NetworkMessagePublisher();
			Mock<Action<TNetworkMessageType, IMessageParameters>> eMessage = new Mock<Action<TNetworkMessageType, IMessageParameters>>();

			//Create some mocks of the delegate parameters
			IMessageParameters parameters = Mock.Of<IMessageParameters>();
			TNetworkMessageType eMessageValue = Mock.Of<TNetworkMessageType>();

			eMessage.CallBase = true;
			eMessage.Setup(x => x(eMessageValue, parameters)); //don't know if this is needed. Never mocked a delegate

			//act
			Subscribe(publisher, eMessage.Object);
			publisher.OnNetworkMessageReceive((dynamic)eMessageValue, parameters); //publish a message

			//assert: Verify that the delegate was invoked after publishing
			eMessage.Verify(x => x(eMessageValue, parameters), Times.Once());
		}

		public static void Subscribe<TMessageType>(INetworkMessageSubscriptionService subService, Action<TMessageType, IMessageParameters> subscriber)
			where TMessageType : INetworkMessage
		{
			//register the subscriber

			if (typeof(TMessageType).IsAssignableFrom(typeof(IEventMessage)))
				INetworkMessageSubcriptionServiceFluentExtensions.With((dynamic)subService.SubscribeTo<TMessageType>(), new OnNetworkEventMessage((Action<IEventMessage, IMessageParameters>)(object)subscriber));

			if (typeof(TMessageType).IsAssignableFrom(typeof(IRequestMessage)))
				INetworkMessageSubcriptionServiceFluentExtensions.With((dynamic)subService.SubscribeTo<TMessageType>(), new OnNetworkRequestMessage((Action<IRequestMessage, IMessageParameters>)(object)subscriber));

			if (typeof(TMessageType).IsAssignableFrom(typeof(IResponseMessage)))
				INetworkMessageSubcriptionServiceFluentExtensions.With((dynamic)subService.SubscribeTo<TMessageType>(), new OnNetworkResponseMessage((Action<IResponseMessage, IMessageParameters>)(object)subscriber));

			if (typeof(TMessageType).IsAssignableFrom(typeof(IStatusMessage)))
				INetworkMessageSubcriptionServiceFluentExtensions.With((dynamic)subService.SubscribeTo<TMessageType>(), new OnNetworkStatusMessage((Action<IStatusMessage, IMessageParameters>)(object)subscriber));
		}
	}
}
