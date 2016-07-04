using GladNet.Message;
using GladNet.Payload;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common.UnitTests
{
	[TestFixture]
	public static class NetworkMessageTests
	{
		//This is complicated. Before you go changing it consult me.
		//It could be done differently, without generic, and with different NUnit and Moq methods
		//but this works for now.
		//UPDATE: I have tried a ValuesAttribute for this. Due to Type checking and Reflection and Generics it won't work. Don't try.
		[Test]
		public static void Test_DeepCopy_With_Children()
		{
			//Call and check all ShallowCopy methods on NetworkMessages.
			Test_DeepCopy((PacketPayload p) => new EventMessage(p));
			Test_DeepCopy((PacketPayload p) => new ResponseMessage(p));
			Test_DeepCopy((PacketPayload p) => new RequestMessage(p));
			Test_DeepCopy((StatusChangePayload p) => new StatusMessage(p));
		}

		private static void Test_DeepCopy<TPayloadType, TMessageType>(Func<TPayloadType, TMessageType> creator)
			where TMessageType : NetworkMessage, IDeepCloneable<NetworkMessage> where TPayloadType : PacketPayload
		{
			//arrange
			TMessageType message = creator(Mock.Of<TPayloadType>());

			//act
			INetworkMessage copiedMessage = message.DeepClone();
			INetworkMessage copiedMessageViaExplict = ((IDeepCloneable)message).DeepClone() as INetworkMessage;

			List<INetworkMessage> messageToTest = new List<INetworkMessage>() { copiedMessage, copiedMessageViaExplict };

			//Assert for each message (both from IDeepCloneable<NetworkMessage> and IDeepCloneable)
			foreach(INetworkMessage m in messageToTest)
			{
				//Check data
				Assert.AreEqual(message.Payload.Data, m.Payload.Data);
				Assert.AreEqual(message.Payload.DataState, m.Payload.DataState);

				//We should also check that the Cloned type is the type expected
				Assert.AreEqual(m.GetType(), typeof(TMessageType));
			}
		}
	}
}
