using GladNet.Common;
using GladNet.Server.Common;
using Logging.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Server.Common.UnitTests
{
	[TestFixture]
	public static class ClientPeerTests
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

            Mock<ClientPeer> peer = new Mock<ClientPeer>(Mock.Of<ILogger>(), sender.Object, Mock.Of<IConnectionDetails>(), Mock.Of<INetworkMessageSubscriptionService>());
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
			Mock<ClientPeer> peer = new Mock<ClientPeer>(MockBehavior.Strict, Mock.Of<ILogger>(), Mock.Of<INetworkMessageSender>(), Mock.Of<IConnectionDetails>(), null);

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
			Mock<ClientPeer> peer = new Mock<ClientPeer>(Mock.Of<ILogger>(), Mock.Of<INetworkMessageSender>(), Mock.Of<IConnectionDetails>(), subService.Object);
            peer.CallBase = true;

			//Makes sure it's created
			//Otherwise Moq won't construct the object
			var r = peer.Object;

			//assert
			subService.Verify(x => x.SubscribeToRequests(It.IsAny<OnNetworkRequestMessage>()), Times.Once());
        }

		private static Mock<ClientPeer> CreateClientPeerMock()
		{
			return new Mock<ClientPeer>(Mock.Of<ILogger>(), Mock.Of<INetworkMessageSender>(), Mock.Of<IConnectionDetails>(), Mock.Of<INetworkMessageSubscriptionService>());
		}
	}
}
