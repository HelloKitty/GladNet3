using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common.Tests
{
	[TestFixture]
	public static class StatusMessageFactoryTests
	{
		[Test]
		public static void Check_Construction_Of_Factory()
		{
			StatusMessageFactory factory = new StatusMessageFactory();
		}

		[Test]
		public static void Check_Construction_Of_Message_With_Valid_Packet()
		{
			//arrange
			PacketPayload payload = new Mock<StatusChangePayload>(MockBehavior.Strict).Object;
			StatusMessageFactory factory = new StatusMessageFactory();
			NetworkMessage message = null;

			//act
			Assert.DoesNotThrow(() => message = factory.With(payload));

			//assert
			Assert.NotNull(message);
			Assert.AreEqual(message.GetType(), typeof(StatusMessage));
			Assert.AreSame(payload, message.Payload.Data);
		}

		[Test]
		public static void Check_Construction_Of_Message_With_Invalid_Packet_Type()
		{
			//arrange
			PacketPayload payload = new Mock<PacketPayload>(MockBehavior.Strict).Object;
			StatusMessageFactory factory = new StatusMessageFactory();
			NetworkMessage message = null;

			//act
			Assert.Throws(typeof(ArgumentException), () => message = factory.With(payload));

			//assert
			Assert.IsNull(message);
		}

		[Test]
		public static void Check_Construction_Of_Message_With_Null_Payload()
		{
			//arrange
			PacketPayload payload = null;
			StatusMessageFactory factory = new StatusMessageFactory();
			NetworkMessage message = null;

			//act
			Assert.Throws(typeof(ArgumentNullException), () => message = factory.With(payload));

			//assert
			Assert.IsNull(message);
			Assert.IsNull(payload);
		}
	}
}
