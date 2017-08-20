using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GladNet.Serializer.Protobuf
{
	[TestFixture]
	public static class ProtobufnetSerializerStrategyTests
	{
		[Test]
		public static void Test_Ctor_Doesnt_Throw()
		{
			//assert
			Assert.DoesNotThrow(() => new ProtobufnetSerializerStrategy());
		}

		[Test]
		public static void Test_Throws_On_Null_Value()
		{
			//arrange
			ProtobufnetSerializerStrategy serializer = new ProtobufnetSerializerStrategy();

			//assert
			Assert.Throws<ArgumentNullException>(() => serializer.Serialize<object>(null));
		}

		[Test]
		[TestCase("Hello")]
		[TestCase("")]
		[TestCase((int)5)]
		[TestCase(5.0f)]
		[TestCase(1.4d)]
		public static void Test_Deserializes_To_Equivalent_Value<TObjectType>(TObjectType obj)
		{
			//arrange
			ProtobufnetSerializerStrategy serializer = new ProtobufnetSerializerStrategy();
			MemoryStream ms = new MemoryStream();

			//act
			ProtoBuf.Serializer.Serialize(ms, obj);

			//assert
			Assert.AreEqual(ms.ToArray(), serializer.Serialize<TObjectType>(obj));

			ms.Dispose();
		}
	}
}
