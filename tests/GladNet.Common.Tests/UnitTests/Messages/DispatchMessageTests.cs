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
	public static class DispatchMessageTests
	{
		[Test]
		public static void Test_Constructor_Properties()
		{
			//arrange
			NetworkMessage nMessage = new ResponseMessage(Mock.Of<PacketPayload>()); //just need a message to test with
			DispatchMessage message = new DispatchMessage(nMessage, NetworkMessage.DeliveryMethod.ReliableOrdered, true, 50);

			//assert
			Assert.AreEqual(message.Channel, 50);
			Assert.AreEqual(message.Encrypted, true);
			Assert.AreEqual(message.DeliveryMethod, NetworkMessage.DeliveryMethod.ReliableOrdered);
			Assert.AreEqual(message.Message, nMessage);
		}
	}
}
