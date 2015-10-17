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
		public static void Test_DeepCopy_With_Children()
		{
			//Call and check all ShallowCopy methods on NetworkMessages.
			Test_DeepCopy((PacketPayload p) => new EventMessage(p));
			Test_DeepCopy((PacketPayload p) => new ResponseMessage(p));
			Test_DeepCopy((PacketPayload p) => new RequestMessage(p));
			Test_DeepCopy((StatusChangePayload p) => new StatusMessage(p));
		}

		public static void Test_DeepCopy<TPayloadType, TMessageType>(Func<TPayloadType, TMessageType> creator)
			where TMessageType : NetworkMessage, IDeepCloneable<NetworkMessage> where TPayloadType : PacketPayload
		{
			//arrange
			TMessageType message = creator(Mock.Of<TPayloadType>());

			//act
			INetworkMessage copiedMessage = message.DeepClone();

			//Assert
			Assert.AreEqual(message.Payload.Data, copiedMessage.Payload.Data);
			Assert.AreEqual(message.Payload.DataState, copiedMessage.Payload.DataState);

			//We should also check that the Cloned type is the type expected
			Assert.AreEqual(copiedMessage.GetType(), typeof(TMessageType));
		}
	}
}
