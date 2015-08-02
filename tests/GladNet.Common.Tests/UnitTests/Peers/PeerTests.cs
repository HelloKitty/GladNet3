using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq.Protected;

namespace GladNet.Common.UnitTests
{
	[TestFixture]
	public static class PeerTests
	{
		[Test]
		public static void Test_Constructor()
		{
			//TODO: When it's finished add test
		}

		[Test]
		public static void Test_Receive_Methods()
		{
			//arrange
			Mock<Peer> peer = new Mock<Peer>(MockBehavior.Loose);
			//This sets it up so that the implementation of concrete methods are available.
			peer.CallBase = true;

			INetworkMessageReceiver receiver = peer.Object;
			Mock<IMessageParameters> parameters = new Mock<IMessageParameters>(MockBehavior.Strict);

			//Create the message types
			Mock<IEventMessage> eventMessage = new Mock<IEventMessage>(MockBehavior.Strict);
			Mock<IResponseMessage> responseMessage = new Mock<IResponseMessage>(MockBehavior.Strict);
			Mock<IRequestMessage> requestMessage = new Mock<IRequestMessage>(MockBehavior.Strict);

			//act
			//Call for event.
			receiver.OnNetworkMessageReceive(eventMessage.Object, parameters.Object);
			receiver.OnNetworkMessageReceive(responseMessage.Object, parameters.Object);
			receiver.OnNetworkMessageReceive(requestMessage.Object, parameters.Object);

			//assert
			//peer.As<INetworkMessageReceiver>().Verify(p => p.OnNetworkMessageReceive(eventMessage.Object, parameters.Object), Times.Once());
			peer.Protected().Verify("OnReceiveEvent", Times.Once(), eventMessage.Object, parameters.Object);
			peer.Protected().Verify("OnReceiveResponse", Times.Once(), responseMessage.Object, parameters.Object);
			peer.Protected().Verify("OnReceiveRequest", Times.Once(), requestMessage.Object, parameters.Object);
		}
	}
}
