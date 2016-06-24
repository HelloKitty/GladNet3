using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GladNet.Serializer.Protobuf
{
	[TestFixture]
	public static class ProtobufnetDeserializerStrategyTests
	{
		[Test]
		public static void Test_Ctor_Doesnt_Throw()
		{
			//assert
			Assert.DoesNotThrow(() => new ProtobufnetDeserializerStrategy());
		}

		[Test]
		public static void Test_Throws_On_Null_Value()
		{
			//arrange
			ProtobufnetDeserializerStrategy deserializer = new ProtobufnetDeserializerStrategy();

			//assert
			Assert.Throws<ArgumentNullException>(() => deserializer.Deserialize<object>(null));
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
			ProtobufnetDeserializerStrategy deserializer = new ProtobufnetDeserializerStrategy();

			MemoryStream ms = new MemoryStream();
			ProtoBuf.Serializer.Serialize(ms, obj);
			ms.Position = 0; //this is needed because it won't rewind the stream

			//assert
			Assert.AreEqual(ProtoBuf.Serializer.Deserialize<TObjectType>(ms), deserializer.Deserialize<TObjectType>(ms.ToArray()));

			ms.Dispose();
		}
	}
}
