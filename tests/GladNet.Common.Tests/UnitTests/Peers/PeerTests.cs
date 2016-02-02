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
			/*public bool VerifyAgainst(IMessageParameters parameters)
			{
				return true;
			}

			public bool VerifyAgainst(bool encrypt, byte channel, DeliveryMethod method)
			{
				return true;
			}*/

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
		public static void Test_Request_Receive_Method()
		{
			//arrange
			Mock<Peer> peer = CreatePeerMock();
			//This sets it up so that the implementation of concrete methods are available.
			peer.CallBase = true;

			INetworkMessageReceiver receiver = peer.Object;
			Mock<IMessageParameters> parameters = new Mock<IMessageParameters>(MockBehavior.Strict);

			//Create the message types
			Mock<IRequestMessage> requestMessage = new Mock<IRequestMessage>(MockBehavior.Strict);

			//act
			//Call for event.
			receiver.OnNetworkMessageReceive(requestMessage.Object, parameters.Object);

			//assert
			peer.Protected().Verify("OnReceiveRequest", Times.Once(), requestMessage.Object, parameters.Object);
		}

		[Test]
		public static void Test_Event_Receive_Method()
		{
			//arrange
			Mock<Peer> peer = CreatePeerMock();
			//This sets it up so that the implementation of concrete methods are available.
			peer.CallBase = true;

			INetworkMessageReceiver receiver = peer.Object;
			Mock<IMessageParameters> parameters = new Mock<IMessageParameters>(MockBehavior.Strict);

			//Create the message types
			Mock<IEventMessage> eventMessage = new Mock<IEventMessage>(MockBehavior.Strict);

			//act
			//Call for event.
			receiver.OnNetworkMessageReceive(eventMessage.Object, parameters.Object);

			//assert
			//peer.As<INetworkMessageReceiver>().Verify(p => p.OnNetworkMessageReceive(eventMessage.Object, parameters.Object), Times.Once());
			peer.Protected().Verify("OnReceiveEvent", Times.Once(), eventMessage.Object, parameters.Object);
		}

		[Test]
		public static void Test_Response_Receive_Method()
		{
			//arrange
			Mock<Peer> peer = CreatePeerMock();
			//This sets it up so that the implementation of concrete methods are available.
			peer.CallBase = true;

			INetworkMessageReceiver receiver = peer.Object;
			Mock<IMessageParameters> parameters = new Mock<IMessageParameters>(MockBehavior.Strict);

			//Create the message types
			Mock<IResponseMessage> responseMessage = new Mock<IResponseMessage>(MockBehavior.Strict);

			//act
			//Call for event.
			receiver.OnNetworkMessageReceive(responseMessage.Object, parameters.Object);

			//assert
			//peer.As<INetworkMessageReceiver>().Verify(p => p.OnNetworkMessageReceive(eventMessage.Object, parameters.Object), Times.Once());
			peer.Protected().Verify("OnReceiveResponse", Times.Once(), responseMessage.Object, parameters.Object);
		}

		[Test]
		public static void Test_OnStatusChange_Method([EnumRange(typeof(NetStatus))] NetStatus status)
		{
			//arrange
			Mock<Peer> peer = CreatePeerMock();
			//This sets it up so that the implementation of concrete methods are available.
			peer.CallBase = true;

			INetworkMessageReceiver receiver = peer.Object;

			//act
			receiver.OnStatusChanged(status);

			//assert
			peer.Protected().Verify("OnStatusChanged", Times.Once(), status);
		}

		[Test]
		public static void Test_Peer_TrySendMessage_Methods(
			[EnumRange(typeof(OperationType))] OperationType opType,
			[EnumRange(typeof(DeliveryMethod))] DeliveryMethod deliveryMethod)
		{
			//arrange
			Mock<Peer> peer = CreatePeerMock();
			Mock<PacketPayload> payload = new Mock<PacketPayload>(MockBehavior.Strict);
			//Enable calling implemented methods
			peer.CallBase = true;

			//act
			if(peer.Object.CanSend(opType))
				Assert.AreNotEqual(peer.Object.TrySendMessage(opType, payload.Object, deliveryMethod), SendResult.Invalid);
			else
				Assert.AreEqual(peer.Object.TrySendMessage(opType, payload.Object, deliveryMethod), SendResult.Invalid);

			//Assert
			//Check that the generic method called the non-generic
			peer.Verify(m => m.TrySendMessage(opType, payload.Object, deliveryMethod, false, 0), Times.Once());
		}

		[Test]
		public static void Test_Peer_TrySendMessageGeneric_Methods([EnumRangeAttribute(typeof(OperationType))] OperationType opType)
		{
			//arrange
			Mock<Peer> peer = CreatePeerMock();
			TestPayloadWithStaticParams payload = new TestPayloadWithStaticParams();
			//Enable calling implemented methods
			peer.CallBase = true;

			//act
			if (peer.Object.CanSend(opType))
				Assert.AreNotEqual(peer.Object.TrySendMessage(opType, payload), SendResult.Invalid);
			else
				Assert.AreEqual(peer.Object.TrySendMessage(opType, payload), SendResult.Invalid);

			//Assert
			//Check that the generic method called the non-generic
			peer.Verify(m => m.TrySendMessage(opType, payload, payload.DeliveryMethod, payload.Encrypted, payload.Channel), Times.Once());
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

		[Test]
		public static void Test_Peer_Recieve_Emulation_Methods_WithFalse()
		{
			//arrange
			Mock<Peer> peer = CreatePeerMock();
			peer.CallBase = true;


			//assert
			//All emulation methods should throw.
			Assert.That(() => peer.Object.EmulateOnStatusChanged(NetStatus.Connected), Throws.TypeOf<InvalidOperationException>());

			Assert.That(() => peer.Object.EmulateOnNetworkMessageReceive(Mock.Of<IEventMessage>(), Mock.Of<IMessageParameters>()), 
				Throws.TypeOf<InvalidOperationException>());

			Assert.That(() => peer.Object.EmulateOnNetworkMessageReceive(Mock.Of<IResponseMessage>(), Mock.Of<IMessageParameters>()), 
				Throws.TypeOf<InvalidOperationException>());

			Assert.That(() => peer.Object.EmulateOnNetworkMessageReceive(Mock.Of<IRequestMessage>(), Mock.Of<IMessageParameters>()), 
				Throws.TypeOf<InvalidOperationException>());
		}

		[Test]
		public static void Test_Peer_Recieve_Emulation_Methods_WithTrue()
		{
			//arrange
			Mock<Peer> peer = CreatePeerMock();

			peer.CallBase = true;
			peer.Object.AllowReceiverEmulation = true;


			//assert (shouldn't throw)
			//Don't remove the asserts. It's best practice to Assert throwing nothing so that potential
			peer.Object.EmulateOnStatusChanged(NetStatus.Connected);
			peer.Object.EmulateOnNetworkMessageReceive(Mock.Of<IEventMessage>(), Mock.Of<IMessageParameters>());
			peer.Object.EmulateOnNetworkMessageReceive(Mock.Of<IResponseMessage>(), Mock.Of<IMessageParameters>());
			peer.Object.EmulateOnNetworkMessageReceive(Mock.Of<IRequestMessage>(), Mock.Of<IMessageParameters>());

			Assert.Pass("Enumation methods didn't throw when emulation was enabled. Passes specification.");
		}
		private static Mock<Peer> CreatePeerMock()
		{
			return new Mock<Peer>(MockBehavior.Loose, Mock.Of<ILogger>(), Mock.Of<INetworkMessageSender>(), Mock.Of<IConnectionDetails>());
		}
	}
}
