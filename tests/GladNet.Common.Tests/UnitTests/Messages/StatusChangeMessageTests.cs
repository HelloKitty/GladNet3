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
	public static class StatusChangeMessageTests
	{
		[Test]
		public static void Test_Construction()
		{
			//arrange
			StatusChangePayload payload = new StatusChangePayload(NetStatus.Disconnected);

			//act
			StatusMessage message = new StatusMessage(payload);

			//assert
			Assert.AreEqual((message.Payload.Data as StatusChangePayload).Status, NetStatus.Disconnected);
		}

		[Test]
		public static void Test_Dispatch()
		{			
			//arrange
			Mock<StatusChangePayload> payload = new Mock<StatusChangePayload>(MockBehavior.Strict);
			Mock<INetworkMessageReceiver> reciever = new Mock<INetworkMessageReceiver>();

			//act
			StatusMessage message = new StatusMessage(payload.Object);
			message.Dispatch(reciever.Object, null);

			//assert
			//We check that the proper method was called.
			reciever.Verify(x => x.OnStatusChanged(payload.Object.Status), Times.Once());
		}
	}
}
