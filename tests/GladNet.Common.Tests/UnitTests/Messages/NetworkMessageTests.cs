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
	public static class NetworkMessageTests
	{
		[Test]
		public static void Test_ShallowCopy_With_Children()
		{
			//Call and check all ShallowCopy methods on NetworkMessages.
			Test_ShallowCopy(p => new EventMessage(p));
			Test_ShallowCopy(p => new ResponseMessage(p));
			Test_ShallowCopy(p => new RequestMessage(p));
		}

		public static void Test_ShallowCopy<TMessageType>(Func<PacketPayload,TMessageType> creator)
			where TMessageType : NetworkMessage, IShallowCloneable<NetworkMessage>
		{
			//arrange
			TMessageType message = creator(Mock.Of<PacketPayload>());

			//act
			INetworkMessage copiedMessage = message.ShallowClone();

			//Assert
			Assert.AreEqual(message.Payload, copiedMessage.Payload);
		}
	}
}
