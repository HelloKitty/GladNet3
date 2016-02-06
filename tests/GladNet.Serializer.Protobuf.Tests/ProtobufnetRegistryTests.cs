using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Serializer.Protobuf.Tests
{
	[TestFixture]
	public static class ProtobufnetRegistryTests
	{
		[Test]
		public static void Test_Ctor_Doesnt_Throw()
		{
			//assert
			Assert.DoesNotThrow(() => new ProtobufnetRegistry());
		}

		[Test]
		public static void Test_Register_Throws_On_Null_Type()
		{
			//arrange
			ProtobufnetRegistry registry = new ProtobufnetRegistry();

			//assert
			Assert.Throws<ArgumentNullException>(() => registry.Register(null));
		}

		[Test]
		[TestCase(typeof(string))]
		[TestCase(typeof(int))]
		[TestCase(typeof(float))]
		[TestCase(typeof(double))]
		[TestCase(typeof(DateTime))]
		public static void Test_Expected_Preregistered_Types_Indicate_They_Arent_Registered_And_Return_False(Type t)
		{
			//arrange
			ProtobufnetRegistry registry = new ProtobufnetRegistry();

			Assert.False(registry.Register(t));
		}
	}
}
