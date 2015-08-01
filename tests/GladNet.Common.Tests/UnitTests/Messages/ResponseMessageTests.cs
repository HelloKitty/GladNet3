﻿using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common.UnitTests
{
	[TestFixture]
	public static class ResponseMessageTests
	{
		private static readonly Random random = new Random();

		[Test]
		public static void Test_Construction()
		{
			//arrange
			Mock<PacketPayload> packet = new Mock<PacketPayload>(MockBehavior.Strict);

			//act
			ResponseMessage message = new ResponseMessage(packet.Object, TestByte());

			//assert
			//Just that it doesn't throw.
		}

		[Test]
		public static void Test_Properties_After_Construction()
		{
			//arrange
			Mock<PacketPayload> packet = new Mock<PacketPayload>(MockBehavior.Strict);
			byte testByte = TestByte();

			//act
			ResponseMessage message = new ResponseMessage(packet.Object, testByte);

			//assert
			Assert.AreSame(packet.Object, message.Payload);
			Assert.AreEqual(testByte, message.ResponseCode);
		}


		[Test]
		[ExpectedException]
		public static void Test_Construction_Null_Packet()
		{
			//act
			new ResponseMessage(null, TestByte());

			//assert
			//Exception should be thrown for null.
		}

		[Test]
		public static void Test_Dispatch()
		{
			//arrange
			Mock<PacketPayload> packet = new Mock<PacketPayload>(MockBehavior.Strict);
			ResponseMessage message = new ResponseMessage(packet.Object, TestByte());
			Mock<IMessageParameters> parameters = new Mock<IMessageParameters>(MockBehavior.Strict);
			Mock<INetworkMessageReceiver> receiever = new Mock<INetworkMessageReceiver>(MockBehavior.Strict);

			//Sets up the method that should be called so it doesn't throw.
			//Also rigs it up so that the two mocks above should be the values provided.
			receiever.Setup((actual) => actual.OnNetworkMessageRecieve(message, parameters.Object));

			//act
			message.Dispatch(receiever.Object, parameters.Object);

			//asset
			receiever.Verify((actual) => actual.OnNetworkMessageRecieve(message, parameters.Object));
		}

		[Test]
		[ExpectedException]
		public static void Test_Dispatch_Null_Receiver()
		{
			//arrange
			Mock<PacketPayload> packet = new Mock<PacketPayload>(MockBehavior.Strict);
			ResponseMessage message = new ResponseMessage(packet.Object, TestByte());
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
			ResponseMessage message = new ResponseMessage(packet.Object, TestByte());
			Mock<INetworkMessageReceiver> receiever = new Mock<INetworkMessageReceiver>(MockBehavior.Strict);

			//Sets up the method that should be called so it doesn't throw.
			//Also rigs it up so that the two mocks above should be the values provided.
			receiever.Setup((actual) => actual.OnNetworkMessageRecieve(message, null));

			//act
			message.Dispatch(receiever.Object, null);

			//assert
			//Exception should be thrown for null.
		}

		private static byte TestByte()
		{
			return (byte)(random.Next() % (byte.MaxValue + 1));
		}
	}
}