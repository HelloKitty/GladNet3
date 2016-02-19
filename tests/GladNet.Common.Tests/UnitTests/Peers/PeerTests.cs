using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq.Protected;
using Logging.Services;

namespace GladNet.Common.Tests
{
	[TestFixture]
	public static class PeerTests
	{
		public class TestPayloadWithStaticParams : PacketPayload, IStaticPayloadParameters
		{
			public bool Encrypted
			{
				get { return true; }
			}

			public byte Channel
			{
				get { return 5; }
			}

			public DeliveryMethod DeliveryMethod
			{
				get { return Common.DeliveryMethod.ReliableUnordered; }
			}
		}


		[Test]
		public static void Test_Constructor()
		{
			//TODO: When it's finished add test
		}

		[Test]
		public static void Test_Peer_TrySendMessage_Methods(
			[EnumRange(typeof(OperationType))] OperationType opType,
			[EnumRange(typeof(DeliveryMethod))] DeliveryMethod deliveryMethod)
		{
			//arrange
			Mock<INetworkMessageSender> sender = new Mock<INetworkMessageSender>();
			sender.Setup(x => x.CanSend(It.IsAny<OperationType>())).Returns(false);

			Mock<Peer> peer = new Mock<Peer>(MockBehavior.Loose, Mock.Of<ILogger>(), sender.Object, Mock.Of<IConnectionDetails>(), Mock.Of<INetworkMessageSubscriptionService>(), Mock.Of<IDisconnectionServiceHandler>());
			Mock<PacketPayload> payload = new Mock<PacketPayload>(MockBehavior.Strict);
			//Enable calling implemented methods
			peer.CallBase = true;

			//act
			SendResult result;
			if(peer.Object.CanSend(opType))
				Assert.AreNotEqual(result = peer.Object.TrySendMessage(opType, payload.Object, deliveryMethod), SendResult.Invalid);
			else
				Assert.AreEqual(result = peer.Object.TrySendMessage(opType, payload.Object, deliveryMethod), SendResult.Invalid);

			//Assert
			//Check that the generic method called the non-generic
			sender.Verify(m => m.TrySendMessage(opType, payload.Object, deliveryMethod, false, 0), 
				result == SendResult.Invalid ? Times.Never() : Times.Once());
		}

		[Test]
		public static void Test_Peer_TrySendMessageGeneric_Methods([EnumRangeAttribute(typeof(OperationType))] OperationType opType)
		{
			//arrange
			Mock<INetworkMessageSender> sender = new Mock<INetworkMessageSender>();
			sender.Setup(x => x.CanSend(It.IsAny<OperationType>())).Returns(false);

			Mock<Peer> peer = new Mock<Peer>(MockBehavior.Loose, Mock.Of<ILogger>(), sender.Object, Mock.Of<IConnectionDetails>(), Mock.Of<INetworkMessageSubscriptionService>(), Mock.Of<IDisconnectionServiceHandler>());
			TestPayloadWithStaticParams payload = new TestPayloadWithStaticParams();
			//Enable calling implemented methods
			peer.CallBase = true;

			//act
			SendResult result;
			if (peer.Object.CanSend(opType))
				Assert.AreNotEqual(result = peer.Object.TrySendMessage(opType, payload), SendResult.Invalid);
			else
				Assert.AreEqual(result = peer.Object.TrySendMessage(opType, payload), SendResult.Invalid);

			//Assert
			//Check that the generic method called the non-generic
			sender.Verify(m => m.TrySendMessage(opType, payload, payload.DeliveryMethod, payload.Encrypted, payload.Channel), 
				result == SendResult.Invalid ? Times.Never() : Times.Once());
		}

		[Test]
		public static void Test_Peer_TrySendMessage_WithNullPacket(
			[EnumRange(typeof(OperationType))] OperationType opToTest,
			[EnumRange(typeof(DeliveryMethod))] DeliveryMethod deliveryMethod)
		{
			//arrange
			Mock<Peer> peer = CreatePeerMock();

			//Enable calling implemented methods
			peer.CallBase = true;

			//assert
			Assert.Throws<ArgumentNullException>(() => peer.Object.TrySendMessage(opToTest, null, deliveryMethod));
		}

		[Test]
		public static void Test_Peer_TrySendMessageGeneric_WithNullPacket([EnumRangeAttribute(typeof(OperationType))] OperationType opToTest)
		{
			//arrange
			Mock<Peer> peer = CreatePeerMock();

			//Enable calling implemented methods
			peer.CallBase = true;

			//asert
			Assert.Throws<ArgumentNullException>(() => peer.Object.TrySendMessage<TestPayloadWithStaticParams>(opToTest, null)); //Call with dynamic as the mocked type fits the generic constraints but we can't compile time prove it.
		}

		private static Mock<Peer> CreatePeerMock()
		{
			return new Mock<Peer>(MockBehavior.Loose, Mock.Of<ILogger>(), Mock.Of<INetworkMessageSender>(), Mock.Of<IConnectionDetails>(), Mock.Of<INetworkMessageSubscriptionService>(), Mock.Of<IDisconnectionServiceHandler>());
		}
	}
}
