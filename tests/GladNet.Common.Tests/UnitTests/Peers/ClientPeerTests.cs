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
		public static void Test_CanSend_IsRequest()
		{
			//arrange
			Mock<ClientPeer> peer = new Mock<ClientPeer>();
			peer.CallBase = true;

			//act
			bool result = peer.Object.CanSend(NetworkMessage.OperationType.Request); //peer.As<INetPeer>().Object.CanSend(NetworkMessage.OperationType.Request);

			//assert
			Assert.IsTrue(result);
		}
	}
}
