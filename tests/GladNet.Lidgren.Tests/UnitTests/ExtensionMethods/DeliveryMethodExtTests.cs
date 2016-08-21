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
	public static class DeliveryMethodExtTests
	{
		[Test]
		//Test the mapping process. We DO NOT compile-time enforce mapping because we don't reference Lidgren dll where our DeliveryMethod is defined.
		[TestCase(NetDeliveryMethod.ReliableOrdered, DeliveryMethod.ReliableOrdered)]
		[TestCase(NetDeliveryMethod.ReliableSequenced, DeliveryMethod.ReliableDiscardStale)]
		[TestCase(NetDeliveryMethod.ReliableUnordered, DeliveryMethod.ReliableUnordered)]
		[TestCase(NetDeliveryMethod.Unreliable, DeliveryMethod.UnreliableAcceptDuplicate)]
		[TestCase(NetDeliveryMethod.UnreliableSequenced, DeliveryMethod.UnreliableDiscardStale)]
		public static void Test_LidgrenDelivery_to_GladNetDelivery(NetDeliveryMethod lidgrenNetDeliveryMethod, DeliveryMethod gladnetDeliveryMethod)
		{
			Assert.AreEqual(gladnetDeliveryMethod, lidgrenNetDeliveryMethod.ToGladNet());
		}

		[Test]
		public static void Test_LidgrenDelivery_to_GladNetDelivery_Invalid_DeliveryMethod()
		{
			//arrange
			NetDeliveryMethod test = NetDeliveryMethod.UnreliableSequenced + 5;

			Assert.False(Enum.IsDefined(typeof(NetDeliveryMethod), test), "Can't use this constant for this test. Please change it.");

			//assert
			Assert.AreEqual(test.ToGladNet(), DeliveryMethod.Unknown);
		}

		[Test]
		//Test the mapping process. We DO NOT compile-time enforce mapping because we don't reference Lidgren dll where our DeliveryMethod is defined.
		[TestCase(NetDeliveryMethod.ReliableOrdered, DeliveryMethod.ReliableOrdered)]
		[TestCase(NetDeliveryMethod.ReliableSequenced, DeliveryMethod.ReliableDiscardStale)]
		[TestCase(NetDeliveryMethod.ReliableUnordered, DeliveryMethod.ReliableUnordered)]
		[TestCase(NetDeliveryMethod.Unreliable, DeliveryMethod.UnreliableAcceptDuplicate)]
		[TestCase(NetDeliveryMethod.UnreliableSequenced, DeliveryMethod.UnreliableDiscardStale)]
		public static void Test_GladNet_to_Lidgren(NetDeliveryMethod lidgrenNetDeliveryMethod, DeliveryMethod gladnetDeliveryMethod)
		{
			Assert.AreEqual(gladnetDeliveryMethod.ToLidgren(), lidgrenNetDeliveryMethod);
		}

		[Test]
		public static void Test_GladNet_to_Lidgren_Invalid_DeliveryMethod()
		{
			//arrange
			DeliveryMethod test = DeliveryMethod.Unknown + 50;

			Assert.False(Enum.IsDefined(typeof(DeliveryMethod), test), "Can't use this constant for this test. Please change it.");

			//assert
			Assert.AreEqual(test.ToLidgren(), NetDeliveryMethod.Unknown);
		}
	}
}
