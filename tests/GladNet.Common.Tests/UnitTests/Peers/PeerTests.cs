﻿using Moq;
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
		public static void Test_OnStatusChange_Method()
		{
			//arrange
			Mock<Peer> peer = CreatePeerMock();
			//This sets it up so that the implementation of concrete methods are available.
			peer.CallBase = true;

			INetworkMessageReceiver receiver = peer.Object;

			//act
			receiver.OnStatusChanged(NetStatus.Disconnected);

			//assert
			peer.Protected().Verify("OnStatusChanged", Times.Once(), NetStatus.Disconnected);
		}

		[Test]
		public static void Test_Peer_TrySendMessage_Methods()
		{
			//arrange
			Mock<Peer> peer = CreatePeerMock();
			Mock<PacketPayload> packet = new Mock<PacketPayload>(MockBehavior.Strict);
			//Enable calling implemented methods
			peer.CallBase = true;

			//act
			foreach (NetworkMessage.OperationType op in Enum.GetValues(typeof(NetworkMessage.OperationType)))
			{
				if(peer.Object.CanSend(op))
					Assert.AreNotEqual(peer.Object.TrySendMessage(op, packet.Object, NetworkMessage.DeliveryMethod.Unknown), NetworkMessage.SendResult.Invalid);
				else
					Assert.AreEqual(peer.Object.TrySendMessage(op, packet.Object, NetworkMessage.DeliveryMethod.Unknown), NetworkMessage.SendResult.Invalid);
			}
		}

		[Test]
		[ExpectedException]
		[TestCase(NetworkMessage.OperationType.Event)]
		[TestCase(NetworkMessage.OperationType.Response)]
		[TestCase(NetworkMessage.OperationType.Request)]
		public static void Test_Peer_TrySendMessage_WithNullPacket(NetworkMessage.OperationType opToTest)
		{
			//arrange
			Mock<Peer> peer = CreatePeerMock();

			//Enable calling implemented methods
			peer.CallBase = true;

			//act
			peer.Object.TrySendMessage(opToTest, null, NetworkMessage.DeliveryMethod.Unknown);

			//expect exception
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public static void Test_Peer_Recieve_Emulation_Methods_WithFalse()
		{
			//arrange
			Mock<Peer> peer = CreatePeerMock();

			peer.CallBase = true;

			//This chain try catch will not throw if one of them doesn't throw. If they all fail it throws so the test passes.
			try
			{
				peer.Object.EmulateOnStatusChanged(NetStatus.Connected);
			}
			catch(InvalidOperationException)
			{
				try
				{
					peer.Object.EmulateOnNetworkMessageReceive(Mock.Of<IEventMessage>(), Mock.Of<IMessageParameters>());
				}
				catch(InvalidOperationException)
				{
					try
					{
						peer.Object.EmulateOnNetworkMessageReceive(Mock.Of<IResponseMessage>(), Mock.Of<IMessageParameters>());
					}
					catch(InvalidOperationException)
					{
						peer.Object.EmulateOnNetworkMessageReceive(Mock.Of<IRequestMessage>(), Mock.Of<IMessageParameters>());
					}
				}
			}
		}

		[Test]
		public static void Test_Peer_Recieve_Emulation_Methods_WithTrue()
		{
			//arrange
			Mock<Peer> peer = CreatePeerMock();

			peer.CallBase = true;
			peer.Object.AllowReceiverEmulation = true;


			//assert (shouldn't throw)
			peer.Object.EmulateOnStatusChanged(NetStatus.Connected);
			peer.Object.EmulateOnNetworkMessageReceive(Mock.Of<IEventMessage>(), Mock.Of<IMessageParameters>());
			peer.Object.EmulateOnNetworkMessageReceive(Mock.Of<IResponseMessage>(), Mock.Of<IMessageParameters>());
			peer.Object.EmulateOnNetworkMessageReceive(Mock.Of<IRequestMessage>(), Mock.Of<IMessageParameters>());
		}
		private static Mock<Peer> CreatePeerMock()
		{
			return new Mock<Peer>(MockBehavior.Loose, Mock.Of<INetworkMessageSender>(), Mock.Of<IConnectionDetails>());
		}
	}
}
