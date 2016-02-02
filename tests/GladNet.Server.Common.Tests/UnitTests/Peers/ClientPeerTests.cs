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
			Mock<ClientPeer> peer = CreateClientPeerMock();
			peer.CallBase = true;

			//act
			bool result = peer.Object.CanSend(opType);

			//assert
			Assert.AreEqual(result, expectedResult);
		}

		private static Mock<ClientPeer> CreateClientPeerMock()
		{
			return new Mock<ClientPeer>(Mock.Of<ILogger>(), Mock.Of<INetworkMessageSender>(), Mock.Of<IConnectionDetails>(), Mock.Of<INetworkMessageSubscriptionService>());
		}
	}
}
