using GladNet.Common;
using GladNet.Server.Common;
using Common.Logging;
using Moq;
using NUnit.Framework;
using System;

namespace GladNet.Server.Common.Tests
{
	[TestFixture]
	public static class ClientPeerSessionTests
	{
		[Test]
		[TestCase(OperationType.Request, false)]
		[TestCase(OperationType.Event, true)]
		[TestCase(OperationType.Response, true)]
		public static void Test_CanSend_IsRequest(OperationType opType, bool expectedResult)
		{
			//arrange
			Mock<INetworkMessageSender> sender = new Mock<INetworkMessageSender>();
			sender.Setup(x => x.CanSend(opType)).Returns(expectedResult); //set this up so it doesn't affect results

			Mock<ClientPeerSession> peer = new Mock<ClientPeerSession>(Mock.Of<ILog>(), sender.Object, Mock.Of<IConnectionDetails>(), Mock.Of<INetworkMessageSubscriptionService>(), Mock.Of<IDisconnectionServiceHandler>());
			peer.CallBase = true;

			//act
			bool result = peer.Object.CanSend(opType);

			//assert
			Assert.AreEqual(result, expectedResult);
		}

		[Test]
		public static void Test_Throws_On_Null_Sub_Service()
		{
			//arrange
			Mock<ClientPeerSession> peer = new Mock<ClientPeerSession>(MockBehavior.Strict, Mock.Of<ILog>(), Mock.Of<INetworkMessageSender>(), Mock.Of<IConnectionDetails>(), null, Mock.Of<IDisconnectionServiceHandler>());

			//assert
			Assert.IsTrue(new Func<bool>(() =>
			{
				try
				{
					var r = peer.Object;

					return false;
				}
				catch(Exception e)
				{
					return e.InnerException.GetType() == typeof(ArgumentNullException);
				}
			}).Invoke(), "Expected it to throw exception from null sub service.");
		}

		[Test]
		public static void Test_Registered_RequestMessage_With_NetMessageSubService()
		{
			//arrange
			Mock<INetworkMessageSubscriptionService> subService = new Mock<INetworkMessageSubscriptionService>(MockBehavior.Loose);
			Mock<ClientPeerSession> peer = new Mock<ClientPeerSession>(Mock.Of<ILog>(), Mock.Of<INetworkMessageSender>(), Mock.Of<IConnectionDetails>(), subService.Object, Mock.Of<IDisconnectionServiceHandler>());
			peer.CallBase = true;

			//Makes sure it's created
			//Otherwise Moq won't construct the object
			var r = peer.Object;

			//assert
			subService.Verify(x => x.SubscribeToRequests(It.IsAny<OnNetworkRequestMessage>()), Times.Once());
		}


		[Test(Author = "Andrew Blakely", Description = "Calling send response should call send service.", TestOf = typeof(ClientPeerSession))]
		public static void Test_SendResposne_Calls_Send_Response_On_NetSend_Service()
		{
			//arrange
			Mock<INetworkMessageSender> sendService = new Mock<INetworkMessageSender>(MockBehavior.Loose);
			PacketPayload payload = Mock.Of<PacketPayload>();

			//set it up to indicate we can send
			sendService.Setup(x => x.CanSend(It.IsAny<OperationType>()))
				.Returns(true);

			Mock<ClientPeerSession> peer = new Mock<ClientPeerSession>(Mock.Of<ILog>(), sendService.Object, Mock.Of<IConnectionDetails>(), Mock.Of<INetworkMessageSubscriptionService>(), Mock.Of<IDisconnectionServiceHandler>());
			peer.CallBase = true;

			//act
			peer.Object.SendResponse(payload, DeliveryMethod.ReliableOrdered, true, 5);

			//assert
			sendService.Verify(x => x.TrySendMessage(OperationType.Response, payload, DeliveryMethod.ReliableOrdered, true, 5), Times.Once());
		}

		[Test(Author = "Andrew Blakely", Description = "Calling send response should call send service.", TestOf = typeof(ClientPeerSession))]
		public static void Test_SendResposne_Calls_Send_Event_On_NetSend_Service()
		{
			//arrange
			Mock<INetworkMessageSender> sendService = new Mock<INetworkMessageSender>(MockBehavior.Loose);
			PacketPayload payload = Mock.Of<PacketPayload>();

			//set it up to indicate we can send
			sendService.Setup(x => x.CanSend(It.IsAny<OperationType>()))
				.Returns(true);

			Mock<ClientPeerSession> peer = new Mock<ClientPeerSession>(Mock.Of<ILog>(), sendService.Object, Mock.Of<IConnectionDetails>(), Mock.Of<INetworkMessageSubscriptionService>(), Mock.Of<IDisconnectionServiceHandler>());
			peer.CallBase = true;

			//act
			peer.Object.SendEvent(payload, DeliveryMethod.ReliableOrdered, true, 5);

			//assert
			sendService.Verify(x => x.TrySendMessage(OperationType.Event, payload, DeliveryMethod.ReliableOrdered, true, 5), Times.Once());
		}

		[Test(Author = "Andrew Blakely", Description = "Calling send response should call send service.", TestOf = typeof(ClientPeerSession))]
		public static void Test_SendResposne_Calls_Send_Event_On_NetSend_Service_With_Generic_Static_Params()
		{
			//arrange
			Mock<INetworkMessageSender> sendService = new Mock<INetworkMessageSender>(MockBehavior.Loose);
			TestPayload payload = new TestPayload();

			//set it up to indicate we can send
			sendService.Setup(x => x.CanSend(It.IsAny<OperationType>()))
				.Returns(true);

			Mock<ClientPeerSession> peer = new Mock<ClientPeerSession>(Mock.Of<ILog>(), sendService.Object, Mock.Of<IConnectionDetails>(), Mock.Of<INetworkMessageSubscriptionService>(), Mock.Of<IDisconnectionServiceHandler>());
			peer.CallBase = true;

			//act
			peer.Object.SendEvent(payload);

			//assert
			sendService.Verify(x => x.TrySendMessage(OperationType.Event, payload), Times.Once());
		}

		[Test(Author = "Andrew Blakely", Description = "Calling send response should call send service.", TestOf = typeof(ClientPeerSession))]
		public static void Test_SendResposne_Calls_Send_Response_On_NetSend_Service_With_Generic_Static_Params()
		{
			//arrange
			Mock<INetworkMessageSender> sendService = new Mock<INetworkMessageSender>(MockBehavior.Loose);
			TestPayload payload = new TestPayload();

			//set it up to indicate we can send
			sendService.Setup(x => x.CanSend(It.IsAny<OperationType>()))
				.Returns(true);

			Mock<ClientPeerSession> peer = new Mock<ClientPeerSession>(Mock.Of<ILog>(), sendService.Object, Mock.Of<IConnectionDetails>(), Mock.Of<INetworkMessageSubscriptionService>(), Mock.Of<IDisconnectionServiceHandler>());
			peer.CallBase = true;

			//act
			peer.Object.SendResponse(payload);

			//assert
			sendService.Verify(x => x.TrySendMessage(OperationType.Response, payload), Times.Once());
		}
	}
}
