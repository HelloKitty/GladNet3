using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FreecraftCore.Serializer;
using NUnit.Framework;
using ProtoBuf;

namespace GladNet.API.Tests.UnitTests
{
	[TestFixture]
	public class ProtobufAdapterTests
	{
		[ProtoContract]
		public class TestProtobufModel
		{
			[ProtoMember(1)]
			public int TestField { get; private set; }

			/// <inheritdoc />
			public TestProtobufModel(int testField)
			{
				TestField = testField;
			}

			public TestProtobufModel()
			{
				
			}
		}

		[Test]
		public static void CanCreateProtobufAdapter()
		{
			//assert
			Assert.DoesNotThrow(() => new ProtobufNetGladNetSerializerAdapter());
		}

		[Test]
		public static void DefaultToNoPrefix()
		{
			//arrange
			ProtobufNetGladNetSerializerAdapter adatper = new ProtobufNetGladNetSerializerAdapter();

			//assert
			Assert.AreEqual(PrefixStyle.None, adatper.PrefixStyle);
		}

		[Test]
		public static void CanSerializerSimpleMessage()
		{
			//assert
			ProtobufNetGladNetSerializerAdapter adatper = new ProtobufNetGladNetSerializerAdapter();

			//act
			byte[] bytes = adatper.Serialize(new TestProtobufModel(5));

			//assert
			Assert.True(bytes.Any());
			Assert.NotNull(bytes);
		}

		[Test]
		public static void CanSerializeNonAsyncUnprefixedSimpleMessage()
		{
			//assert
			ProtobufNetGladNetSerializerAdapter adatper = new ProtobufNetGladNetSerializerAdapter();

			//act
			byte[] bytes = adatper.Serialize(new TestProtobufModel(5));
			TestProtobufModel result = adatper.Deserialize<TestProtobufModel>(bytes);

			//assert
			Assert.NotNull(result);
			Assert.AreEqual(5, result.TestField);
		}

		[Test]
		public static void CanSerializeNonAsyncPrefixedSimpleMessage()
		{
			//assert
			ProtobufNetGladNetSerializerAdapter adatper = new ProtobufNetGladNetSerializerAdapter(PrefixStyle.Fixed32);
			ProtobufNetGladNetSerializerAdapter adatperUnPrefixed = new ProtobufNetGladNetSerializerAdapter();

			//act
			byte[] bytes = adatper.Serialize(new TestProtobufModel(5));
			byte[] bytesUnPrefixed = adatperUnPrefixed.Serialize(new TestProtobufModel(5));
			TestProtobufModel result = adatper.Deserialize<TestProtobufModel>(bytes);

			//assert
			Assert.True(bytes.Length > bytesUnPrefixed.Length);
			Assert.NotNull(result);
			Assert.AreEqual(5, result.TestField);
		}

		[Test]
		public static void CanSerializeAsyncPrefixedSimpleMessage()
		{
			//assert
			ProtobufNetGladNetSerializerAdapter adatper = new ProtobufNetGladNetSerializerAdapter(PrefixStyle.Fixed32);

			//act
			byte[] bytes = adatper.Serialize(new TestProtobufModel(5));
			TestProtobufModel result = adatper.DeserializeAsync<TestProtobufModel>(new AsyncFreecraftCoreSerializationTests.BytesReadableTest(bytes), CancellationToken.None).Result;

			//assert
			Assert.NotNull(result);
			Assert.AreEqual(5, result.TestField);
		}
	}
}
