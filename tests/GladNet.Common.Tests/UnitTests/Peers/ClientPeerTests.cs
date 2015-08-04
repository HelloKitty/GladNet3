using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common.UnitTests.Peers
{
	[TestFixture]
	public static class ClientPeerTests
	{
		[Test]
		[TestCase(NetworkMessage.OperationType.Request, true)]
		[TestCase(NetworkMessage.OperationType.Event, false)]
		[TestCase(NetworkMessage.OperationType.Response, false)]
		public static void Test_CanSend_IsRequest(NetworkMessage.OperationType opType, bool expectedResult)
		{
			//arrange
			Mock<ClientPeer> peer = new Mock<ClientPeer>();
			peer.CallBase = true;

			//act
			bool result = peer.Object.CanSend(opType);

			//assert
			Assert.AreEqual(result, expectedResult);
		}
	}
}
