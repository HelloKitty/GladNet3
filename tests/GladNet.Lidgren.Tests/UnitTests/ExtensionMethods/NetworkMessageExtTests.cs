using Lidgren.Network;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GladNet.Common.Extensions;

namespace GladNet.Common.UnitTests
{
	[TestFixture]
	public static class NetworkMessageExtTests
	{
		[Test]
		public static void Test_LidgrenDelivery_to_GladNetDelivery()
		{
			//arrange
			NetDeliveryMethod ReliableOrdered = NetDeliveryMethod.ReliableOrdered;
			NetDeliveryMethod ReliableSequenced = NetDeliveryMethod.ReliableSequenced;
			NetDeliveryMethod ReliableUnordered = NetDeliveryMethod.ReliableUnordered;
			NetDeliveryMethod Unreliable = NetDeliveryMethod.Unreliable;
			NetDeliveryMethod UnreliableSequenced = NetDeliveryMethod.UnreliableSequenced;

			//assert
			Assert.AreEqual(ReliableOrdered.NetDeliveryMethodTypeToGladNetDeliveryType(), DeliveryMethod.ReliableOrdered);
			Assert.AreEqual(ReliableSequenced.NetDeliveryMethodTypeToGladNetDeliveryType(), DeliveryMethod.ReliableDiscardStale);
			Assert.AreEqual(ReliableUnordered.NetDeliveryMethodTypeToGladNetDeliveryType(), DeliveryMethod.ReliableUnordered);
			Assert.AreEqual(Unreliable.NetDeliveryMethodTypeToGladNetDeliveryType(), DeliveryMethod.UnreliableAcceptDuplicate);
			Assert.AreEqual(UnreliableSequenced.NetDeliveryMethodTypeToGladNetDeliveryType(), DeliveryMethod.UnreliableDiscardStale);
		}

		[Test]
		public static void Test_LidgrenDelivery_to_GladNetDelivery_Invalid_DeliveryMethod()
		{
			//arrange
			NetDeliveryMethod test = NetDeliveryMethod.UnreliableSequenced + 5;

			//assert
			Assert.AreEqual(test.NetDeliveryMethodTypeToGladNetDeliveryType(), DeliveryMethod.Unknown);
		}
	}
}
