using GladNet.Payload;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Serializer.Tests
{
	[TestFixture]
	public static class GladNetSerializationIncludeAttributeTests
	{
		[Test]
		public static void Test_Child_Attribute_Of_Include_Can_Be_Found()
		{
			//assert
			Assert.IsTrue(typeof(TestClass).GetCustomAttribute<GladNetSerializationIncludeAttribute>() != null);
			Assert.IsTrue(typeof(TestClass).GetCustomAttribute<GladNetSerializationIncludeAttribute>().GetType() == typeof(TestClindAttribute));
		}

		[TestClindAttribute(GladNetIncludeIndex.Index1, typeof(PacketPayload), false)]
		public class TestClass
		{

		}

		public class TestClindAttribute : GladNetSerializationIncludeAttribute
		{
			public TestClindAttribute(GladNetIncludeIndex includeIndex, Type type, bool isForDerived = true) 
				: base(includeIndex, type, isForDerived)
			{
			}
		}
	}
}
