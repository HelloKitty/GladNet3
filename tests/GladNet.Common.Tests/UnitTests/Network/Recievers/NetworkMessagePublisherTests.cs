using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common.Tests
{
	[TestFixture(TestOf = typeof(NetworkMessagePublisher))]
	public static class NetworkMessagePublisherTests
	{
		[Test]
		public static void Test_Throws_Exception_On_Null_Subscribe()
		{
			//arrange
			NetworkMessagePublisher publisher = new NetworkMessagePublisher();

			//Assert: All should throw on null.
			Assert.Throws<ArgumentNullException>(() => publisher.SubscribeToEvents(null));
			Assert.Throws<ArgumentNullException>(() => publisher.SubscribeToResponses(null));
			Assert.Throws<ArgumentNullException>(() => publisher.SubscribeToRequests(null));
			Assert.Throws<ArgumentNullException>(() => publisher.SubscribeToStatusChanges(null));
		}

		[Test]
		public static void Test_Doesnt_Throw_On_Valid_Subscription()
		{
			//arrange
			NetworkMessagePublisher publisher = new NetworkMessagePublisher();

			//Assert: All should throw on null.
			Assert.DoesNotThrow(() => publisher.SubscribeToEvents((x, y) => { }));
			Assert.DoesNotThrow(() => publisher.SubscribeToResponses((x, y) => { }));
			Assert.DoesNotThrow(() => publisher.SubscribeToRequests((x, y) => { }));
			Assert.DoesNotThrow(() => publisher.SubscribeToStatusChanges((x, y) => { }));
		}
	}
}
