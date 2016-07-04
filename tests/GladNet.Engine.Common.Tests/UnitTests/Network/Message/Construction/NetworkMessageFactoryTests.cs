using GladNet.Engine.Common;
using GladNet.Message;
using GladNet.Payload;
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
	public static class NetworkMessageFactoryTests
	{
		[Test(Author = "Andrew Blakely", Description = "Tests that the ctor doesn't throw.", TestOf = typeof(NetworkMessageFactory))]
		public static void Test_Ctor_Doesnt_Throw()
		{
			//assert
			Assert.DoesNotThrow(() => new NetworkMessageFactory());
		}

		/* Test for
		* throwing when
		* payload passed
		* is null
		*/

		[Test(Author = "Andrew Blakely", Description = "Tests that factory throws on null payload.", TestOf = typeof(NetworkMessageFactory))]
		public static void Test_Throws_Exception_On_Null_Payload_Event()
		{
			//arrange
			NetworkMessageFactory factory = new NetworkMessageFactory();

			//assert
			Assert.Throws<ArgumentNullException>(() => factory.CreateEventMessage(null));
		}

		[Test(Author = "Andrew Blakely", Description = "Tests that factory throws on null payload.", TestOf = typeof(NetworkMessageFactory))]
		public static void Test_Throws_Exception_On_Null_Payload_Status()
		{
			//arrange
			NetworkMessageFactory factory = new NetworkMessageFactory();

			//assert
			Assert.Throws<ArgumentNullException>(() => factory.CreateStatusMessage(null));
		}

		[Test(Author = "Andrew Blakely", Description = "Tests that factory throws on null payload.", TestOf = typeof(NetworkMessageFactory))]
		public static void Test_Throws_Exception_On_Null_Payload_Response()
		{
			//arrange
			NetworkMessageFactory factory = new NetworkMessageFactory();

			//assert
			Assert.Throws<ArgumentNullException>(() => factory.CreateResponseMessage(null));
		}

		[Test(Author = "Andrew Blakely", Description = "Tests that factory throws on null payload.", TestOf = typeof(NetworkMessageFactory))]
		public static void Test_Throws_Exception_On_Null_Payload_Request()
		{
			//arrange
			NetworkMessageFactory factory = new NetworkMessageFactory();

			//assert
			Assert.Throws<ArgumentNullException>(() => factory.CreateRequestMessage(null));
		}

		/* Test for
		* not throwing
		* when valid payload
		* is passed
		*/

		[Test(Author = "Andrew Blakely", Description = "Tests that factory produces non-null network message.", TestOf = typeof(NetworkMessageFactory))]
		public static void Test_Works_On_Valid_Payload_Event()
		{
			//arrange
			NetworkMessageFactory factory = new NetworkMessageFactory();

			//act
			EventMessage message = factory.CreateEventMessage(Mock.Of<PacketPayload>());

			//assert
			Assert.NotNull(message);
		}

		[Test(Author = "Andrew Blakely", Description = "Tests that factory produces non-null network message.", TestOf = typeof(NetworkMessageFactory))]
		public static void Test_Works_On_Valid_Payload_Status()
		{
			//arrange
			NetworkMessageFactory factory = new NetworkMessageFactory();

			//act
			StatusMessage message = factory.CreateStatusMessage(Mock.Of<StatusChangePayload>());

			//assert
			Assert.NotNull(message);
		}

		[Test(Author = "Andrew Blakely", Description = "Tests that factory produces non-null network message.", TestOf = typeof(NetworkMessageFactory))]
		public static void Test_Works_On_Valid_Payload_Request()
		{
			//arrange
			NetworkMessageFactory factory = new NetworkMessageFactory();

			//act
			RequestMessage message = factory.CreateRequestMessage(Mock.Of<PacketPayload>());

			//assert
			Assert.NotNull(message);
		}

		[Test(Author = "Andrew Blakely", Description = "Tests that factory produces non-null network message.", TestOf = typeof(NetworkMessageFactory))]
		public static void Test_Works_On_Valid_Payload_Response()
		{
			//arrange
			NetworkMessageFactory factory = new NetworkMessageFactory();

			//act
			ResponseMessage message = factory.CreateResponseMessage(Mock.Of<PacketPayload>());

			//assert
			Assert.NotNull(message);
		}
	}
}
