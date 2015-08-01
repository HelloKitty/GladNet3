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
	public static class EventMessageTests
	{
		[Test]
		public static void Test_Construction()
		{
			//arrange
			Mock<PacketPayload> packet = new Mock<PacketPayload>(MockBehavior.Strict);

			//act
			EventMessage message = new EventMessage(packet.Object);

			//assert
			//Just that it doesn't throw
		}

		[Test]
		public static void Test_Properties_After_Construction()
		{
			//arrange
			Mock<PacketPayload> packet = new Mock<PacketPayload>(MockBehavior.Strict);

			//act
			EventMessage message = new EventMessage(packet.Object);

			//assert
			Assert.AreSame(packet.Object, message.Payload);
		}


		[Test]
		[ExpectedException]
		public static void Test_Construction_Null_Packet()
		{
			//act
			new EventMessage(null);

			//assert
			//Exception should be thrown for null.
		}

		[Test]
		public static void Test_Dispatch()
		{
			//arrange
			Mock<PacketPayload> packet = new Mock<PacketPayload>(MockBehavior.Strict);
			EventMessage message = new EventMessage(packet.Object);
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
		[ExpectedException]
		public static void Test_Dispatch_Null_Receiver()
		{
			//arrange
			Mock<PacketPayload> packet = new Mock<PacketPayload>(MockBehavior.Strict);
			EventMessage message = new EventMessage(packet.Object);
			Mock<IMessageParameters> parameters = new Mock<IMessageParameters>(MockBehavior.Strict);

			//act
			message.Dispatch(null, parameters.Object);

			//assert
			//Exception should be thrown for null.
		}

		[Test]
		[ExpectedException]
		public static void Test_Dispatch_Null_Parameters()
		{
			//arrange
			Mock<PacketPayload> packet = new Mock<PacketPayload>(MockBehavior.Strict);
			EventMessage message = new EventMessage(packet.Object);
			Mock<INetworkMessageReceiver> receiever = new Mock<INetworkMessageReceiver>(MockBehavior.Strict);

			//Sets up the method that should be called so it doesn't throw.
			//Also rigs it up so that the two mocks above should be the values provided.
			receiever.Setup((actual) => actual.OnNetworkMessageReceive(message, null));

			//act
			message.Dispatch(receiever.Object, null);

			//assert
			//Exception should be thrown for null.
		}
	}
}
