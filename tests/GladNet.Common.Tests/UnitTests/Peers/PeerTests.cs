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
		public static void Test_Request_Receive_Method()
		{
			//arrange
			Mock<Peer> peer = new Mock<Peer>(MockBehavior.Loose);
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
			Mock<Peer> peer = new Mock<Peer>(MockBehavior.Loose);
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
			Mock<Peer> peer = new Mock<Peer>(MockBehavior.Loose);
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
		public static void Test_Peer_TrySendMessage_Methods()
		{
			//arrange
			Mock<Peer> peer = new Mock<Peer>(MockBehavior.Loose);
			Mock<PacketPayload> packet = new Mock<PacketPayload>(MockBehavior.Strict);
			//Enable calling implemented methods
			peer.CallBase = true;

			//We build a dictionary of mocks to test specific methods.
			//We are going to use .Net 4.5 dynamic feature to test.
			Dictionary<NetworkMessage.OperationType, dynamic> dictOfParams = new Dictionary<NetworkMessage.OperationType, dynamic>()
			{
				{ NetworkMessage.OperationType.Request, new Mock<IRequestPayload>().Object },
				{ NetworkMessage.OperationType.Response, new Mock<IResponsePayload>().Object },
				{ NetworkMessage.OperationType.Event, new Mock<IEventPayload>().Object }
			};

			//act
			foreach(NetworkMessage.OperationType op in Enum.GetValues(typeof(NetworkMessage.OperationType)))
			{
				//Asserts
				if (peer.Object.CanSend(op))
				{
					Assert.AreNotEqual(peer.Object.TrySendMessage(op, packet.Object, NetworkMessage.DeliveryMethod.Unknown), NetworkMessage.SendResult.Invalid);
					Assert.AreNotEqual(peer.Object.TrySendMessage(packet.Object, dictOfParams[op], NetworkMessage.DeliveryMethod.Unknown), NetworkMessage.SendResult.Invalid);
				}
				else
				{
					//This uses DLR/dynamic a .Net 4.5 feature to call the proper overload. This is only for easy testing.
					Assert.AreEqual(peer.Object.TrySendMessage(packet.Object, dictOfParams[op], NetworkMessage.DeliveryMethod.Unknown), NetworkMessage.SendResult.Invalid);
					Assert.AreEqual(peer.Object.TrySendMessage(op, packet.Object, NetworkMessage.DeliveryMethod.Unknown), NetworkMessage.SendResult.Invalid);
				}
			}
		}

		[Test]
		[ExpectedException]
		public static void Test_Peer_TrySendMessage_Response_WithNullPacket()
		{
			//arrange
			Mock<Peer> peer = new Mock<Peer>(MockBehavior.Loose);
			//Mock<PacketPayload> packet = new Mock<PacketPayload>(MockBehavior.Strict);
			Mock<IResponsePayload> parameters = new Mock<IResponsePayload>(MockBehavior.Strict);
			//Enable calling implemented methods
			peer.CallBase = true;

			//act
			peer.Object.TrySendMessage(null, parameters.Object, NetworkMessage.DeliveryMethod.Unknown);

			//expect exception
		}

		[Test]
		[ExpectedException]
		public static void Test_Peer_TrySendMessage_Event_WithNullPacket()
		{
			//arrange
			Mock<Peer> peer = new Mock<Peer>(MockBehavior.Loose);
			//Mock<PacketPayload> packet = new Mock<PacketPayload>(MockBehavior.Strict);
			Mock<IEventPayload> parameters = new Mock<IEventPayload>(MockBehavior.Strict);
			//Enable calling implemented methods
			peer.CallBase = true;

			//act
			peer.Object.TrySendMessage(null, parameters.Object, NetworkMessage.DeliveryMethod.Unknown);

			//expect exception
		}

		[Test]
		[ExpectedException]
		public static void Test_Peer_TrySendMessage_Request_WithNullPacket()
		{
			//arrange
			Mock<Peer> peer = new Mock<Peer>(MockBehavior.Loose);
			//Mock<PacketPayload> packet = new Mock<PacketPayload>(MockBehavior.Strict);
			Mock<IRequestPayload> parameters = new Mock<IRequestPayload>(MockBehavior.Strict);
			//Enable calling implemented methods
			peer.CallBase = true;

			//act
			peer.Object.TrySendMessage(null, parameters.Object, NetworkMessage.DeliveryMethod.Unknown);

			//expect exception
		}

		[Test]
		[ExpectedException]
		public static void Test_Peer_TrySendMessage_NoParams_WithNullPacket()
		{
			//arrange
			Mock<Peer> peer = new Mock<Peer>(MockBehavior.Loose);
			//Enable calling implemented methods
			peer.CallBase = true;

			//act
			peer.Object.TrySendMessage(NetworkMessage.OperationType.Request, null, NetworkMessage.DeliveryMethod.Unknown);

			//expect exception
		}

		[Test]
		public static void Test_Peer_TrySendMessage_WithNullParameters()
		{
			//arrange
			Mock<Peer> peer = new Mock<Peer>(MockBehavior.Loose);
			Mock<PacketPayload> packet = new Mock<PacketPayload>(MockBehavior.Strict);
			//Mock<IResponsePayload> parameters = new Mock<IResponsePayload>(MockBehavior.Strict);
			//Enable calling implemented methods
			peer.CallBase = true;

			//We expect this to suceed and be handled. Potentially it should be logged I guess.
			//act
			peer.Object.TrySendMessage(packet.Object, (IResponsePayload)null, NetworkMessage.DeliveryMethod.Unknown);
			peer.Object.TrySendMessage(packet.Object, (IRequestPayload)null, NetworkMessage.DeliveryMethod.Unknown);
			peer.Object.TrySendMessage(packet.Object, (IEventPayload)null, NetworkMessage.DeliveryMethod.Unknown);

			//Just assert that no exceptions occured.
		}
	}
}
