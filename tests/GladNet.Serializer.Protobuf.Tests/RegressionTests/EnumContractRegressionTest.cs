using NUnit.Framework;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GladNet.Serializer.Protobuf.Tests
{
	[TestFixture]
	public static class EnumContractRegressionTest
	{
		[Test]
		public static void Test_Enum_Causes_Error_When_Registered_With_Contract()
		{
			//arrange
			TestEnum val = TestEnum.Yep;
			byte[] bytes = null;


			//act
			//Register it
			new ProtobufnetRegistry().Register(typeof(TestEnum));

			MemoryStream ms = new MemoryStream();

			ProtoBuf.Serializer.Serialize(ms, val);
			ms.Position = 0;

			bytes = ms.ToArray();
			ms.Position = 0;

			TestEnum deserializedValue = ProtoBuf.Serializer.Deserialize<TestEnum>(ms);

			Assert.AreEqual(val, deserializedValue);
			Assert.IsTrue(ProtoBuf.Meta.RuntimeTypeModel.Default.CanSerialize(typeof(TestEnum)));
		}

		[Test]
		public static void Test_Enum_Causes_Error_When_Registered_With_Contract_Test_With_Class()
		{
			//arrange
			TestClass val = new TestClass();
			byte[] bytes = null;


			//act
			//Register it
			new ProtobufnetRegistry().Register(typeof(TestClass));

			MemoryStream ms = new MemoryStream();

			ProtoBuf.Serializer.Serialize(ms, val);
			ms.Position = 0;

			bytes = ms.ToArray();
			ms.Position = 0;

			TestClass deserializedValue = ProtoBuf.Serializer.Deserialize<TestClass>(ms);

			Assert.AreEqual(val.TestEnumVal, deserializedValue.TestEnumVal);
			Assert.IsTrue(ProtoBuf.Meta.RuntimeTypeModel.Default.CanSerialize(typeof(TestClass)));
		}

		[GladNetSerializationContract]
		public class TestClass
		{
			[GladNetMember(GladNetDataIndex.Index1)]
			public TestEnum TestEnumVal = TestEnum.Yep;
		}

		[GladNetSerializationContract]
		public enum TestEnum : byte
		{
			Test,
			Values,
			Yep
		}
	}
}
