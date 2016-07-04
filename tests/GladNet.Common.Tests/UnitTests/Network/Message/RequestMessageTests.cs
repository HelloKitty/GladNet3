using GladNet.Payload;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common.UnitTests
{
	[TestFixture]
	public static class RequestMessageTests
	{
		[Test]
		public static void Test_Construction()
		{
			//arrange
			Mock<PacketPayload> packet = new Mock<PacketPayload>(MockBehavior.Strict);

			//act
			RequestMessage message = new RequestMessage(packet.Object);

			//assert
			//Just that it doesn't throw
		}

		[Test]
		public static void Test_Properties_After_Construction()
		{
			//arrange
			Mock<PacketPayload> packet = new Mock<PacketPayload>(MockBehavior.Strict);

			//act
			RequestMessage message = new RequestMessage(packet.Object);

			//assert
			Assert.AreSame(packet.Object, message.Payload.Data);
		}


		[Test]
		public static void Test_Construction_Null_Packet()
		{
			//arrange

			//act
			Assert.Throws<ArgumentNullException>(() => new RequestMessage(null));

			//assert
			//Exception should be thrown for null.
		}

		[Test]
		public static void Test_Dispatch()
		{
			//arrange
			Mock<PacketPayload> packet = new Mock<PacketPayload>(MockBehavior.Strict);
			RequestMessage message = new RequestMessage(packet.Object);
			Mock<IMessageParameters> parameters = new Mock<IMessageParameters>(MockBehavior.Strict);
			Mock<INetworkMessageReceiver> receiever = new Mock<INetworkMessageReceiver>(MockBehavior.Strict);

			//Sets up the method that should be called so it doesn't throw.
			//Also rigs it up so that the two mocks above should be the values provided.
			receiever.Setup((actual) => actual.OnNetworkMessageReceive(message, parameters.Object));

			//act
			message.Dispatch(receiever.Object, parameters.Object);

			//asset
			receiever.Verify((actual) => actual.OnNetworkMessageReceive(message, parameters.Object));
		}

		[Test]
		public static void Test_Dispatch_Null_Receiver()
		{
			//arrange
			Mock<PacketPayload> packet = new Mock<PacketPayload>(MockBehavior.Strict);
			RequestMessage message = new RequestMessage(packet.Object);
			Mock<IMessageParameters> parameters = new Mock<IMessageParameters>(MockBehavior.Strict);

			//assert
			//Exception should be thrown for null.
			Assert.Throws<ArgumentNullException>(() => message.Dispatch(null, parameters.Object));
		}

		[Test]
		public static void Test_Dispatch_Null_Parameters()
		{
			//arrange
			Mock<PacketPayload> packet = new Mock<PacketPayload>(MockBehavior.Strict);
			RequestMessage message = new RequestMessage(packet.Object);
			Mock<INetworkMessageReceiver> receiever = new Mock<INetworkMessageReceiver>(MockBehavior.Strict);

			//Sets up the method that should be called so it doesn't throw.
			//Also rigs it up so that the two mocks above should be the values provided.
			receiever.Setup((actual) => actual.OnNetworkMessageReceive(message, null));

			//assert
			//Exception should be thrown for null.
			Assert.Throws<ArgumentNullException>(() => message.Dispatch(receiever.Object, null));
		}

		[Test]
		public static void Test_Event_Message_Routing_Stack_Empty_On_Initial_Creation()
		{
			//arrange
			Mock<PacketPayload> packet = new Mock<PacketPayload>(MockBehavior.Strict);
			RequestMessage message = new RequestMessage(packet.Object);

			//assert
			Assert.IsNull(message.Peek());
			Assert.IsFalse(message.isMessageRoutable); //shouldn't be able to rout
		}

		[Test]
		public static void Test_Event_Message_Routing_Stack_Has_Pushed_Value()
		{
			//arrange
			Mock<PacketPayload> packet = new Mock<PacketPayload>(MockBehavior.Strict);
			RequestMessage message = new RequestMessage(packet.Object);

			//act
			message.Push(5);

			//assert
			Assert.NotNull(message.Peek());
			Assert.True(message.isMessageRoutable);
			Assert.AreEqual(5, message.Peek());
		}

		[Test]
		public static void Test_Event_Message_Routing_Stack_Has_Pushed_Values()
		{
			//arrange
			Mock<PacketPayload> packet = new Mock<PacketPayload>(MockBehavior.Strict);
			RequestMessage message = new RequestMessage(packet.Object);

			//act
			message.Push(5);
			message.Push(4);
			message.Push(3);

			//assert
			Assert.NotNull(message.Peek());
			Assert.True(message.isMessageRoutable);

			Assert.AreEqual(3, message.Pop());
			Assert.AreEqual(4, message.Pop());
			Assert.AreEqual(5, message.Pop());

			Assert.IsNull(message.Peek());
			Assert.IsFalse(message.isMessageRoutable); //shouldn't be able to rout
		}

		[Test]
		public static void Test_Event_Message_Routing_Stack_Can_Export_To_Other_Message()
		{
			//arrange
			Mock<PacketPayload> packet = new Mock<PacketPayload>(MockBehavior.Strict);
			RequestMessage message = new RequestMessage(packet.Object);

			//act
			message.Push(5);
			message.Push(4);
			message.Push(3);

			ResponseMessage rMessage = new ResponseMessage(packet.Object);

			//export routing stack
			message.ExportRoutingDataTo(rMessage);

			List<IRoutableMessage> Messages = new List<IRoutableMessage>() { rMessage, message };

			foreach (IRoutableMessage m in Messages)
			{
				Assert.NotNull(m.Peek());
				Assert.True(m.isMessageRoutable);

				Assert.AreEqual(3, m.Pop());
				Assert.AreEqual(4, m.Pop());
				Assert.AreEqual(5, m.Pop());

				Assert.IsNull(m.Peek());
				Assert.IsFalse(m.isMessageRoutable);
			}
		}
	}
}
