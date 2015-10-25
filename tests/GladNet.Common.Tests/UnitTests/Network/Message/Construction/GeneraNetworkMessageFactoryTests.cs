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
	public static class GeneraNetworklMessageFactoryTests
	{
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public static void Test_Constructor_For_Factory_With_Null()
		{
			GeneralNetworkMessageFactory factory = new GeneralNetworkMessageFactory(null);
		}

		[Test]
		public static void Test_Constructor_With_Valid_Func()
		{
			GeneralNetworkMessageFactory factory = new GeneralNetworkMessageFactory(p => new EventMessage(p));
		}

		[Test]
		public static void Test_Creation_With_Valid_Payload()
		{
			//Arrange
			GeneralNetworkMessageFactory factory = new GeneralNetworkMessageFactory(p => new EventMessage(p));
			PacketPayload payload = Mock.Of<PacketPayload>();

			//act
			NetworkMessage message = factory.With(payload);

			//assert
			Assert.NotNull(message);
			Assert.AreSame(message.Payload.Data, payload);
			Assert.AreEqual(typeof(EventMessage), message.GetType());
		}
	}
}
