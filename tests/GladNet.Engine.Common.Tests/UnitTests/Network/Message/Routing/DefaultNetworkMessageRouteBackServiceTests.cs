using Common.Logging;
using GladNet.Common;
using GladNet.Message;
using GladNet.Payload;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Engine.Common
{
	[TestFixture]
	public static class DefaultNetworkMessageRouteBackServiceTests
	{
		[Test]
		public static void Test_Ctor_Doesnt_Throw_On_Non_Null_Parameters()
		{
			//assert: Doesn't throw
			Assert.DoesNotThrow(() => new DefaultNetworkMessageRouteBackService(Mock.Of<IAUIDService<INetPeer>>(), Mock.Of<ILog>()));
		}

		[Test]
		public static void Test_Ctor_Throws_On_Null_First_Parameter()
		{
			//assert: Doesn't throw
			Assert.Throws<ArgumentNullException>(() => new DefaultNetworkMessageRouteBackService(null, Mock.Of<ILog>()));
		}

		[Test]
		public static void Test_Ctor_Throws_On_Null_Second_Parameter()
		{
			//assert: Doesn't throw
			Assert.Throws<ArgumentNullException>(() => new DefaultNetworkMessageRouteBackService(Mock.Of<IAUIDService<INetPeer>>(), null));
		}

		[Test]
		public static void Test_Service_Indicates_Invalid_On_Messages_With_No_Routing_Stack()
		{
			//arrange
			INetworkMessageRouteBackService service = new DefaultNetworkMessageRouteBackService(Mock.Of<IAUIDService<INetPeer>>(), Mock.Of<ILog>());

			//assert
			Assert.AreEqual(SendResult.Invalid, service.Route(Mock.Of<IRequestMessage>(), Mock.Of<IMessageParameters>()));
		}

		[Test]
		public static void Test_Service_Enables_Routeback_On_Messages_With_RoutingStack()
		{
			//arrange
			INetworkMessageRouteBackService service = new DefaultNetworkMessageRouteBackService(new AUIDServiceCollection<INetPeer>(5), Mock.Of<ILog>());

			//act: Push some ints onto the routing stack
			RequestMessage message = new RequestMessage(Mock.Of<PacketPayload>());
			message.Push(1);

			//assert
			Assert.True(message.isMessageRoutable);

			//now route attempt
			SendResult result = service.Route(message, Mock.Of<IMessageParameters>());

			//assert: message is no longer routable
			Assert.False(message.isMessageRoutable);
		}

		[Test]
		public static void Test_Service_SucessResult_When_RequestMessage_Has_Routing_Stack_And_Peer_Is_Available()
		{
			RequestMessage message = new RequestMessage(Mock.Of<PacketPayload>());
			message.Push(1);

			Test_Service_SucessResult_When_Message_Has_Routing_Stack_And_Peer_Is_Available<RequestMessage, IRequestMessage>(message);
		}

		[Test]
		public static void Test_Service_SucessResult_When_ResponseMessage_Has_Routing_Stack_And_Peer_Is_Available()
		{
			ResponseMessage message = new ResponseMessage(Mock.Of<PacketPayload>());
			message.Push(1);

			Test_Service_SucessResult_When_Message_Has_Routing_Stack_And_Peer_Is_Available<ResponseMessage, IResponseMessage>(message);
		}

		public static void Test_Service_SucessResult_When_Message_Has_Routing_Stack_And_Peer_Is_Available<TMessageType, TMessageTypeInterface>(TMessageType message)
			where TMessageType : IOperationTypeMappable, INetworkMessage, IRoutableMessage, TMessageTypeInterface
			where TMessageTypeInterface : IOperationTypeMappable, INetworkMessage, IRoutableMessage
		{
			//arrange
			Mock<INetPeer> peer = new Mock<INetPeer>(MockBehavior.Loose);

			Mock<INetworkMessageRouterService> routerService = new Mock<INetworkMessageRouterService>(MockBehavior.Loose);


			routerService.Setup(p => p.TryRouteMessage(It.IsAny<TMessageTypeInterface>(), It.IsAny<DeliveryMethod>(), It.IsAny<bool>(), It.IsAny<byte>()))
				.Returns(SendResult.Sent);

			//setup the send
			peer.SetupGet(p => p.NetworkSendService)
				.Returns(routerService.Object);

			AUIDServiceCollection<INetPeer> peerCollection = new AUIDServiceCollection<INetPeer>(5) { { 1, peer.Object } };
			INetworkMessageRouteBackService service = new DefaultNetworkMessageRouteBackService(peerCollection, Mock.Of<ILog>());

			//assert
			Assert.True(message.isMessageRoutable);
			Assert.AreEqual(peer.Object.NetworkSendService.TryRouteMessage(message, (DeliveryMethod)500, true), SendResult.Sent);

			//now route attempt
			SendResult result = service.Route(message, (new Mock<IMessageParameters>(MockBehavior.Loose)).Object);

			//assert: message is no longer routable
			Assert.False(message.isMessageRoutable);

			//Assert that the result was sent too
			Assert.AreEqual(SendResult.Sent, result);
		}

		[Test]
		public static void Test_Service_SuccessResult_When_Payload_Routed_With_IRoutingDetails()
		{
			//arrange
			Mock<INetPeer> peer = new Mock<INetPeer>(MockBehavior.Loose);

			RequestMessage message = new RequestMessage(Mock.Of<PacketPayload>());
			message.Push(1);

			Mock<INetworkMessageRouterService> routerService = new Mock<INetworkMessageRouterService>(MockBehavior.Loose);

			routerService.Setup(p => p.TryRouteMessage(It.IsAny<IRequestMessage>(), It.IsAny<DeliveryMethod>(), It.IsAny<bool>(), It.IsAny<byte>()))
				.Returns(SendResult.Sent);

			//setup the send
			peer.SetupGet(p => p.NetworkSendService)
				.Returns(routerService.Object);

			AUIDServiceCollection<INetPeer> peerCollection = new AUIDServiceCollection<INetPeer>(5) { { 1, peer.Object } };
			INetworkMessageRouteBackService service = new DefaultNetworkMessageRouteBackService(peerCollection, Mock.Of<ILog>());

			//assert
			Assert.AreEqual(SendResult.Sent, service.RouteRequest(Mock.Of<PacketPayload>(), message, DeliveryMethod.ReliableOrdered, true, 5));
		}
	}
}
