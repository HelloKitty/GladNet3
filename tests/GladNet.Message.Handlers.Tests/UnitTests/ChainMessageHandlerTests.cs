using GladLive.Common;
using GladNet.Common;
using GladNet.Engine.Common;
using GladNet.Message;
using GladNet.Message.Handlers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladLive.Common.Tests
{
	[TestFixture]
	public static class ChainMessageHandlerTests
	{
		[Test]
		public static void Test_Ctor_Doesnt_Throw()
		{
			//arrange
			Assert.DoesNotThrow(() => new ChainMessageHandlerStrategy<INetPeer, INetworkMessage>());
		}

		[Test]
		public static void Test_Can_Add_New_Handler()
		{
			//arrange
			var chain = new ChainMessageHandlerStrategy<INetPeer, INetworkMessage>();
			Mock<IMessageHandler<INetPeer, INetworkMessage>> handler = new Mock<IMessageHandler<INetPeer, INetworkMessage>>();

			//act
			bool result = chain.Register(handler.Object);

			//assert
			Assert.IsTrue(result, "Couldn't add a handler to the chain.");
		}

		[Test]
		public static void Test_Indicates_Add_Failure_On_Null()
		{
			//arrange
			var chain = new ChainMessageHandlerStrategy<INetPeer, INetworkMessage>();

			//act
			bool result = chain.Register(null);

			//assert
			Assert.IsFalse(result, "Was able to add a null handler.");
		}

		//Can't do this test because we can't cast
		[Test]
		public static void Test_Chain_Handler_Calls_Handle_Methods_On_Handler()
		{
			//arrange
			var chain = new ChainMessageHandlerStrategy<INetPeer, INetworkMessage>();
			Mock<IMessageHandler<INetPeer, INetworkMessage>> handler = new Mock<IMessageHandler<INetPeer, INetworkMessage>>();

			handler.Setup(x => x.TryProcessMessage(It.IsAny<INetworkMessage>(), It.IsAny<IMessageParameters>(), It.IsAny<INetPeer>()))
				.Returns(true);

			//act
			bool result = chain.Register(handler.Object);
			Assert.IsTrue(result, "Couldn't add a handler to the chain.");

			chain.TryProcessMessage(Mock.Of<INetworkMessage>(), Mock.Of<IMessageParameters>(), Mock.Of<INetPeer>());

			//assert
			handler.Verify(x => x.TryProcessMessage(It.IsAny<INetworkMessage>(), It.IsAny<IMessageParameters>(), It.IsAny<INetPeer>()), Times.Once());
		}

		//Can't do this test because we can't cast
		[Test]
		public static void Test_Chain_Handler_Calls_Handle_Methods_On_Handlers_Multiple()
		{
			//arrange
			var chain = new ChainMessageHandlerStrategy<INetPeer, INetworkMessage>();

			Mock<IMessageHandler<INetPeer, INetworkMessage>> handler1 = new Mock<IMessageHandler<INetPeer, INetworkMessage>>();

			handler1.Setup(x => x.TryProcessMessage(It.IsAny<INetworkMessage>(), It.IsAny<IMessageParameters>(), It.IsAny<INetPeer>()))
				.Returns(false);

			Mock<IMessageHandler<INetPeer, INetworkMessage>> handler2 = new Mock<IMessageHandler<INetPeer, INetworkMessage>>();

			handler2.Setup(x => x.TryProcessMessage(It.IsAny<INetworkMessage>(), It.IsAny<IMessageParameters>(), It.IsAny<INetPeer>()))
				.Returns(false);

			//act
			bool result = chain.Register(handler1.Object);
			Assert.IsTrue(result, "Couldn't add a handler to the chain.");

			result = chain.Register(handler2.Object);
			Assert.IsTrue(result, "Couldn't add a handler to the chain.");

			chain.TryProcessMessage(Mock.Of<INetworkMessage>(), Mock.Of<IMessageParameters>(), Mock.Of<INetPeer>());

			//assert
			handler1.Verify(x => x.TryProcessMessage(It.IsAny<INetworkMessage>(), It.IsAny<IMessageParameters>(), It.IsAny<INetPeer>()), Times.Once());
			handler2.Verify(x => x.TryProcessMessage(It.IsAny<INetworkMessage>(), It.IsAny<IMessageParameters>(), It.IsAny<INetPeer>()), Times.Once());
		}
	}
}
