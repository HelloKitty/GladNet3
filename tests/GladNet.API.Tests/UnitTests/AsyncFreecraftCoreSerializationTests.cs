using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FreecraftCore.Serializer;
using NUnit.Framework;

namespace GladNet
{
	[TestFixture]
	public sealed class AsyncFreecraftCoreSerializationTests
	{
		[Test]
		public void Test_Can_Serialize_Async()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestSerializableType>();
			serializer.Compile();

			//act
			byte[] bytesTask = serializer.SerializeAsync(new TestSerializableType(50, "Hello!")).Result;

			//assert
			Assert.NotNull(bytesTask);
			Assert.IsNotEmpty(bytesTask);
		}

		[Test]
		public void Test_Can_Deserialize_Async()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestSerializableType>();
			serializer.Compile();

			//act
			byte[] bytesTask = serializer.SerializeAsync(new TestSerializableType(50, "Hello!")).Result;
			TestSerializableType result = serializer.DeserializeAsync<TestSerializableType>(new AsyncWireReaderBytesReadableAdapter(new BytesReadableTest(bytesTask))).Result;

			//assert
			Assert.NotNull(bytesTask);
			Assert.IsNotEmpty(bytesTask);

			Assert.NotNull(result);
			Assert.AreEqual(50, result.TestInt);
			Assert.AreEqual("Hello!", result.TestString);
		}

		[Test]
		public void Test_Can_Deserialize_Async_One_Sized_Array()
		{
			//arrange
			SerializerService serializer = new SerializerService();
			serializer.RegisterType<TestSerializableType2>();
			serializer.Compile();

			//act
			byte[] bytesTask = serializer.SerializeAsync(new TestSerializableType2(TestSerializableType2.TestEnum.One, new byte[1] { 7 })).Result;
			Assert.NotNull(bytesTask);
			Assert.IsNotEmpty(bytesTask);

			TestSerializableType2 result = serializer.DeserializeAsync<TestSerializableType2>(new AsyncWireReaderBytesReadableAdapter(new BytesReadableTest(bytesTask))).Result;

			//assert
			Assert.NotNull(result);
			Assert.True(result.TestOneSizedArray.Length == 1);
			Assert.AreEqual(TestSerializableType2.TestEnum.One, result.EnumVal);
			Assert.AreEqual(7, result.TestOneSizedArray[0]);
		}

		public class BytesReadableTest : IBytesReadable
		{
			private byte[] Bytes { get; }

			private int Position { get; set; }

			/// <inheritdoc />
			public BytesReadableTest(byte[] bytes)
			{
				Bytes = bytes;
			}
 
			/// <inheritdoc />
			public async Task<int> ReadAsync(byte[] buffer, int start, int count, CancellationToken token)
			{
				Console.WriteLine($"Reading Start: {start} Count: {count} Position: {Position} OriginalByteLength: {Bytes.Length}");

				for(int j = 0, i = Position + start; i < Position + start + count; j++, i++)
					buffer[j] = Bytes[i];

				Position += count;

				return count;
			}

			/// <inheritdoc />
			public Task ClearReadBuffers()
			{
				return Task.CompletedTask;
			}
		}


		[WireDataContract]
		public class TestSerializableType
		{
			[WireMember(1)]
			public int TestInt { get; private set; }

			[WireMember(2)]
			public string TestString { get; private set; }

			/// <inheritdoc />
			public TestSerializableType(int testInt, string testString)
			{
				TestInt = testInt;
				TestString = testString;
			}

			protected TestSerializableType()
			{
				
			}
		}

		[WireDataContract]
		public class TestSerializableType2
		{
			public enum TestEnum : byte
			{
				One = 1
			}
			
			[WireMember(1)]
			public TestEnum EnumVal { get; }

			[SendSize(SendSizeAttribute.SizeType.Byte)]
			[WireMember(2)]
			public byte[] TestOneSizedArray { get; }

			/// <inheritdoc />
			public TestSerializableType2(TestEnum enumVal, byte[] testOneSizedArray)
			{
				EnumVal = enumVal;
				TestOneSizedArray = testOneSizedArray;
			}

			private TestSerializableType2()
			{
				
			}
		}
	}
}
