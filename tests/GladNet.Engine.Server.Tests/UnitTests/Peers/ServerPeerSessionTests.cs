using GladNet.Common;
using Common.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladNet.Server.Common;
using GladNet.Engine.Common;
using GladNet.Engine.Server;

namespace GladNet.Server.Common.Tests
{
	[TestFixture(TestOf = typeof(ServerPeerSession))]
	public static class ServerPeerSessionTests
	{
		[Test(TestOf = typeof(ServerPeerSession))]
		public static void Test_Ctor_Doesnt_Throw_On_Non_Null_Dependencies()
		{
			//assert
			Assert.DoesNotThrow(() => { var r = new Mock<ServerPeerSession>(Mock.Of<ILog>(), Mock.Of<INetworkMessageRouterService>(), Mock.Of<IConnectionDetails>(), Mock.Of<INetworkMessageSubscriptionService>(), Mock.Of<IDisconnectionServiceHandler>(), Mock.Of<INetworkMessageRouteBackService>()).Object; });
		}

		[Test(Author = "Andrew Blakely", Description = "Should only be able to send events and responses", TestOf = typeof(ServerPeerSession))]
		[TestCase(OperationType.Request, false)]
		[TestCase(OperationType.Event, true)]
		[TestCase(OperationType.Response, true)]
		public static void Test_CanSend_IsRequest(OperationType opType, bool expectedResult)
		{
			//arrange
			Mock<INetworkMessageRouterService> sender = new Mock<INetworkMessageRouterService>();
			sender.Setup(x => x.CanSend(opType)).Returns(expectedResult); //set this up so it doesn't affect results

			Mock<ServerPeerSession> peer = new Mock<ServerPeerSession>(Mock.Of<ILog>(), sender.Object, Mock.Of<IConnectionDetails>(), Mock.Of<INetworkMessageSubscriptionService>(), Mock.Of<IDisconnectionServiceHandler>(), Mock.Of<INetworkMessageRouteBackService>());
			peer.CallBase = true;

			//act
			bool result = peer.Object.CanSend(opType);

			//assert
			Assert.AreEqual(result, expectedResult);
		}

		[Test]
		public static void Test_Registered_RequestMessage_With_NetMessageSubService()
		{
			//arrange
			Mock<INetworkMessageSubscriptionService> subService = new Mock<INetworkMessageSubscriptionService>(MockBehavior.Loose);
			Mock<ServerPeerSession> peer = new Mock<ServerPeerSession>(Mock.Of<ILog>(), Mock.Of<INetworkMessageRouterService>(), Mock.Of<IConnectionDetails>(), subService.Object, Mock.Of<IDisconnectionServiceHandler>(), Mock.Of<INetworkMessageRouteBackService>());
			peer.CallBase = true;

			//Makes sure it's created
			//Otherwise Moq won't construct the object
			var r = peer.Object;

			//assert
			subService.Verify(x => x.SubscribeToRequests(It.IsAny<OnNetworkRequestMessage>()), Times.Once());
		}
	}
}
