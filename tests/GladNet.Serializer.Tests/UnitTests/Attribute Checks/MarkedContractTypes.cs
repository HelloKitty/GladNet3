using GladNet.Common;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using GladNet.Serializer;

namespace GladNet.Common.Tests
{
	[TestFixture]
	public static class MarkedContractTypes
	{
		//These types absolutely MUST be marked with contracts.
		//The entire library expects them to be and serialization will fail, at some point, if they aren't
		//in the flow of execution.
		[Test]
		[TestCase(typeof(NetworkMessage))]
		[TestCase(typeof(StatusMessage))]
		[TestCase(typeof(RequestMessage))]
		[TestCase(typeof(ResponseMessage))]
		[TestCase(typeof(EventMessage))]
		[TestCase(typeof(PacketPayload))]
		[TestCase(typeof(NetSendable<>))]
		[TestCase(typeof(NetSendableState))]
		[TestCase(typeof(StatusChangePayload))]
		[TestCase(typeof(NetStatus))]
		public static void Check_If_Types_Are_Marked(Type typeExpectedToBeMarked)
		{
			//Arrange
			bool isMarked = typeExpectedToBeMarked.GetCustomAttribute<GladNetSerializationContractAttribute>(false) != null;

			//assert
			Assert.True(isMarked);
		}
	}
}
