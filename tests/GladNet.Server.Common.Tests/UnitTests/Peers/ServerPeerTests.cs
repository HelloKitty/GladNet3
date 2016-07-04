using GladNet.Common;
using Common.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladNet.Payload;

namespace GladNet.Server.Common.Tests
{
	[TestFixture(TestOf = typeof(ClientPeer))]
	public static class ServerPeerTests
	{
		[Test(Author = "Andrew Blakely", Description = "Should only be able to send requests.", TestOf = typeof(ClientPeer))]
		[TestCase(OperationType.Request, true)]
		[TestCase(OperationType.Event, false)]
		[TestCase(OperationType.Response, false)]
		public static void Test_CanSend_IsRequest(OperationType opType, bool expectedResult)
		{
			//arrange
			Mock<INetworkMessageRouterService> sender = new Mock<INetworkMessageRouterService>();
			sender.Setup(x => x.CanSend(opType)).Returns(expectedResult); //set this up so it doesn't affect results

			Mock<ClientPeer> peer = new Mock<ClientPeer>(Mock.Of<ILog>(), sender.Object, Mock.Of<IConnectionDetails>(), Mock.Of<INetworkMessageSubscriptionService>(), Mock.Of<IDisconnectionServiceHandler>());
			peer.CallBase = true;

			//act
			bool result = peer.Object.CanSend(opType);

			//assert
			Assert.AreEqual(result, expectedResult);
		}

		[Test(Author = "Andrew Blakely", TestOf = typeof(ClientPeer))]
		public static void Test_Ctor_Doesnt_Throw_On_Non_Null_Dependencies()
		{
			//Assert
			Assert.DoesNotThrow(() => { var r = new Mock<ClientPeer>(Mock.Of<ILog>(), Mock.Of<INetworkMessageRouterService>(), Mock.Of<IConnectionDetails>(), Mock.Of<INetworkMessageSubscriptionService>(), Mock.Of<IDisconnectionServiceHandler>()).Object; } );
		}

		[Test(Author = "Andrew Blakely", Description = nameof(ClientPeer) + " should be listening for events and responses.", TestOf = typeof(ClientPeer))]
		public static void Test_Registered_EventMessage_With_NetMessageSubService()
		{
			//arrange
			Mock<INetworkMessageSubscriptionService> subService = new Mock<INetworkMessageSubscriptionService>(MockBehavior.Loose);
			Mock<ClientPeer> peer = new Mock<ClientPeer>(Mock.Of<ILog>(), Mock.Of<INetworkMessageRouterService>(), Mock.Of<IConnectionDetails>(), subService.Object, Mock.Of<IDisconnectionServiceHandler>());
			peer.CallBase = true;

			//Makes sure it's created
			//Otherwise Moq won't construct the object
			var r = peer.Object;

			//assert
			subService.Verify(x => x.SubscribeToEvents(It.IsAny<OnNetworkEventMessage>()), Times.Once());
		}


		[Test(Author = "Andrew Blakely", Description = nameof(ClientPeer) + " should be listening for events and responses.", TestOf = typeof(ClientPeer))]
		public static void Test_Registered_ResponseMessage_With_NetMessageSubService()
		{
			//arrange
			Mock<INetworkMessageSubscriptionService> subService = new Mock<INetworkMessageSubscriptionService>(MockBehavior.Loose);
			Mock<ClientPeer> peer = new Mock<ClientPeer>(Mock.Of<ILog>(), Mock.Of<INetworkMessageRouterService>(), Mock.Of<IConnectionDetails>(), subService.Object, Mock.Of<IDisconnectionServiceHandler>());
			peer.CallBase = true;

			//Makes sure it's created
			//Otherwise Moq won't construct the object
			var r = peer.Object;

			//assert
			subService.Verify(x => x.SubscribeToResponses(It.IsAny<OnNetworkResponseMessage>()), Times.Once());
		}

		//don't test that peer isn't subbed to request messages. They may listen for invalids

		[Test(Author = "Andrew Blakely", Description = "Calling send response should call send service.", TestOf = typeof(ClientPeer))]
		public static void Test_SendResposne_Calls_Send_Request_On_NetSend_Service_With_Generic_Static_Params()
		{
			//arrange
			Mock<INetworkMessageRouterService> sendService = new Mock<INetworkMessageRouterService>(MockBehavior.Loose);
			TestPayload payload = new TestPayload();

			//set it up to indicate we can send
			sendService.Setup(x => x.CanSend(It.IsAny<OperationType>()))
				.Returns(true);

			Mock<ClientPeer> peer = new Mock<ClientPeer>(Mock.Of<ILog>(), sendService.Object, Mock.Of<IConnectionDetails>(), Mock.Of<INetworkMessageSubscriptionService>(), Mock.Of<IDisconnectionServiceHandler>());
			peer.CallBase = true;

			//act
			peer.Object.SendRequest(payload);

			//assert
			sendService.Verify(x => x.TrySendMessage(OperationType.Request, payload), Times.Once());
		}

		[Test(Author = "Andrew Blakely", Description = "Calling send response should call send service.", TestOf = typeof(ClientPeer))]
		public static void Test_SendResposne_Calls_Send_Request_On_NetSend_Service()
		{
			//arrange
			Mock<INetworkMessageRouterService> sendService = new Mock<INetworkMessageRouterService>(MockBehavior.Loose);
			PacketPayload payload = Mock.Of<PacketPayload>();

			//set it up to indicate we can send
			sendService.Setup(x => x.CanSend(It.IsAny<OperationType>()))
				.Returns(true);

			Mock<ClientPeer> peer = new Mock<ClientPeer>(Mock.Of<ILog>(), sendService.Object, Mock.Of<IConnectionDetails>(), Mock.Of<INetworkMessageSubscriptionService>(), Mock.Of<IDisconnectionServiceHandler>());
			peer.CallBase = true;

			//act
			peer.Object.SendRequest(payload, DeliveryMethod.ReliableOrdered, true, 5);

			//assert
			sendService.Verify(x => x.TrySendMessage(OperationType.Request, payload, DeliveryMethod.ReliableOrdered, true, 5), Times.Once());
		}
	}
}
