using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
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

			//assert
			Assert.False(registry.Register(t));
		}

		[Test]
		public static void Test_Can_Register_Marked_Types()
		{
			//arrange
			ProtobufnetRegistry registry = new ProtobufnetRegistry();

			//assert
			Assert.IsTrue(registry.Register(typeof(TestEmptyClass)));
			Assert.IsTrue(registry.Register(typeof(TestWithMember)));
			Assert.IsTrue(registry.Register(typeof(TestWithNestedSerializableType)));
		}

		[Test]
		public static void Test_Can_Register_Type_Will_Not_Fail_If_Try_To_Reregister()
		{
			//arrange
			ProtobufnetRegistry registry = new ProtobufnetRegistry();

			//assert
			Assert.IsTrue(registry.Register(typeof(TestEmptyClass)));
			Assert.IsTrue(registry.Register(typeof(TestEmptyClass)));
		}

		[Test]
		public static void Test_Doesnt_Fail_On_Circular_Graph()
		{
			//arrange
			ProtobufnetRegistry registry = new ProtobufnetRegistry();

			//assert
			Assert.DoesNotThrow(() => registry.Register(typeof(TestCircularGraph)));
		}

		[Test]
		public static void Test_Can_Serialize_Then_Deserialize_Registered_Type()
		{
			//arrange
			TestWithNestedSerializableType typeToTest = new TestWithNestedSerializableType();
			typeToTest.IntField = 5;
			typeToTest.SomeClassField = new TestWithNestedSerializableType.SomeClass() { SomeField = 8 };
			ProtobufnetRegistry registry = new ProtobufnetRegistry();
			typeToTest.ShouldntSerialize = 50643;

			//act
			registry.Register(typeof(TestWithNestedSerializableType));

			//Serialize it
			MemoryStream ms = new MemoryStream();

			ProtoBuf.Serializer.Serialize(ms, typeToTest);
			ms.Position = 0; //need to reset the stream

			TestWithNestedSerializableType deserializedType = ProtoBuf.Serializer.Deserialize<TestWithNestedSerializableType>(ms);

			//assert
			Assert.NotNull(deserializedType);
			Assert.AreEqual(typeToTest.IntField, deserializedType.IntField);
			Assert.NotNull(deserializedType.SomeClassField);
			Assert.AreEqual(typeToTest.SomeClassField.SomeField, deserializedType.SomeClassField.SomeField);
			Assert.AreNotEqual(typeToTest.ShouldntSerialize, deserializedType.ShouldntSerialize);
		}

		[Test]
		public static void Test_Can_Register_Type_With_Includes()
		{
			//arrange
			ProtobufnetRegistry registry = new ProtobufnetRegistry();

			Assert.IsTrue(registry.Register(typeof(TestBaseType)));
		}

		[Test]
		public static void Test_Can_Register_Backwards_Include_Type()
		{
			//arrange
			ProtobufnetRegistry registry = new ProtobufnetRegistry();
			//TestChildTypeWithInclude value = new TestChildTypeWithInclude() { IntField = 5 };

			//assert
			Assert.IsTrue(registry.Register(typeof(TestChildTypeWithInclude)));
		}

		[Test]
		public static void Test_Can_Serialize_Then_Deserialize_Backwards_Included_Type()
		{
			//arrange
			TestBaseType typeToTest = new TestChildTypeWithInclude(5068);
			ProtobufnetRegistry registry = new ProtobufnetRegistry();

			//act
			registry.Register(typeof(TestBaseType));
			registry.Register(typeof(TestChildTypeWithInclude2));
			registry.Register(typeof(TestChildTypeWithInclude));
			registry.Register(typeof(TestChildType));
			

			//Serialize it
			MemoryStream ms = new MemoryStream();

			ProtoBuf.Serializer.Serialize(ms, typeToTest);
			ms.Position = 0; //need to reset the stream

			TestChildTypeWithInclude deserializedType = ProtoBuf.Serializer.Deserialize<TestBaseType>(ms)
				as TestChildTypeWithInclude;

			//assert
			Assert.NotNull(deserializedType);
			Assert.AreEqual(deserializedType.GetType(), typeToTest.GetType());
			Assert.AreEqual(((TestChildTypeWithInclude)typeToTest).IntField2, deserializedType.IntField2);
		}

		public static void Test_Deserializes_To_Right_Type()
		{
			//ProtobufnetRegistry registry = new ProtobufnetRegistry();

			//registry.Register(
		}

		[GladNetSerializationContract]
		public class TestEmptyClass
		{

		}

		[GladNetSerializationContract]
		public class TestWithMember
		{
			[GladNetMember(GladNetDataIndex.Index1)]
			public int IntField;
		}

		[GladNetSerializationContract]
		public class TestWithNestedSerializableType
		{
			[GladNetSerializationContract]
			public class SomeClass
			{
				[GladNetMember(GladNetDataIndex.Index2)]
				public int SomeField;
			}

			[GladNetMember(GladNetDataIndex.Index1)]
			public int IntField;

			[GladNetMember(GladNetDataIndex.Index2)]
			public SomeClass SomeClassField;

			public int ShouldntSerialize;
		}

		[GladNetSerializationContract]
		public class TestCircularGraph
		{
			[GladNetSerializationContract]
			public class SomeClass
			{
				[GladNetMember(GladNetDataIndex.Index2)]
				private int SomeField;

				public TestCircularGraph CircleField;
			}

			[GladNetMember(GladNetDataIndex.Index1)]
			public int IntField;

			[GladNetMember(GladNetDataIndex.Index2)]
			public SomeClass SomeClassField;
		}

		[GladNetSerializationContract]
		[GladNetSerializationInclude(GladNetIncludeIndex.Index1, typeof(TestChildType))]
		public class TestBaseType
		{

		}

		[GladNetSerializationContract]
		public class TestChildType : TestBaseType
		{
			[GladNetMember(GladNetDataIndex.Index1)]
			public int IntField1;
		}

		[GladNetSerializationContract]
		[GladNetSerializationInclude(GladNetIncludeIndex.Index2, typeof(TestBaseType), false)]
		public class TestChildTypeWithInclude : TestBaseType
		{
			[GladNetMember(GladNetDataIndex.Index3)]
			public int IntField2 { get; private set; }

			public TestChildTypeWithInclude(int val)
			{
				IntField2 = val;
			}

			public TestChildTypeWithInclude()
			{

			}
		}

		[GladNetSerializationContract]
		[GladNetSerializationInclude(GladNetIncludeIndex.Index3, typeof(TestBaseType), false)]
		public class TestChildTypeWithInclude2 : TestBaseType
		{
			[GladNetMember(GladNetDataIndex.Index2)]
			public int IntField5;
		}
	}
}
