using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common.UnitTests
{
	[TestFixture]
	public static class NetSendableTests
	{
		[Test]
		public static void Test_Construction()
		{
			//arrange
			Mock<PacketPayload> payload = new Mock<PacketPayload>(MockBehavior.Strict);

			//act
			NetSendable<PacketPayload> netSendablePayload = new NetSendable<PacketPayload>(payload.Object);

			//assert
			//No exceptions.
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public static void Test_Construction_Null_TData()
		{
			//arrange

			//act
			NetSendable<PacketPayload> netSendablePayload = new NetSendable<PacketPayload>(null);

			//assert
			//expect an exception
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		[TestCase("Serialize")]
		[TestCase("Deserialize")]
		[TestCase("Decrypt")]
		[TestCase("Encrypt")]
		public static void Test_Methods_With_Null(string methodName)
		{
			//arrange
			Mock<PacketPayload> payload = new Mock<PacketPayload>(MockBehavior.Strict);
			NetSendable<PacketPayload> netSendable = new NetSendable<PacketPayload>(payload.Object);

			//act
			try
			{
				//This will throw a reflection exception so we get the innerexception.
				netSendable.GetType().GetMethod(methodName).Invoke(netSendable, new object[1] { null });
			}
			catch (Exception e)
			{
				//Translates the reflection exception to the actual exception
				throw e.InnerException;
			}

			//assert
			//Should throw an exception.
		}

		#region State Valid Operation Tests
		[Test]
		public static void Test_Serialization()
		{
			//arrange
			Mock<PacketPayload> payload = new Mock<PacketPayload>(MockBehavior.Strict);
			NetSendable<PacketPayload> netSendable = new NetSendable<PacketPayload>(payload.Object);
			Mock<ISerializer> serializer = new Mock<ISerializer>(MockBehavior.Strict);

			//We setup the serializer so it returns an empty non-null array.
			serializer.Setup(x => x.Serialize(It.IsAny<PacketPayload>()))
				.Returns(new byte[0]);

			//act
			bool result = netSendable.Serialize(serializer.Object);

			//assert
			Assert.AreEqual(result, true);
			serializer.Verify(x => x.Serialize(payload.Object), Times.Once());
			Assert.AreEqual(netSendable.DataState, NetSendableState.Serialized);
		}


		[Test]
		public static void Test_Deserialization()
		{
			//arrange
			Mock<PacketPayload> payload = new Mock<PacketPayload>(MockBehavior.Strict);
			NetSendable<PacketPayload> netSendable = new NetSendable<PacketPayload>(payload.Object);
			Mock<IDeserializer> deserializer = new Mock<IDeserializer>(MockBehavior.Strict);

			ChangeDataStateProperty(netSendable, NetSendableState.Serialized);

			//We setup the serializer so it returns an empty non-null array.
			deserializer.Setup(x => x.Deserialize<PacketPayload>(It.IsAny<byte[]>()))
				.Returns(Mock.Of<PacketPayload>());

			//act
			bool result = netSendable.Deserialize(deserializer.Object);

			//assert
			Assert.AreEqual(result, true);
			deserializer.Verify(x => x.Deserialize<PacketPayload>(It.IsAny<byte[]>()), Times.Once());
			Assert.AreEqual(netSendable.DataState, NetSendableState.Default);
		}

		[Test]
		public static void Test_Encryption()
		{
			//arrange
			Mock<PacketPayload> payload = new Mock<PacketPayload>(MockBehavior.Strict);
			NetSendable<PacketPayload> netSendable = new NetSendable<PacketPayload>(payload.Object);
			Mock<IEncryptor> encryptor = new Mock<IEncryptor>(MockBehavior.Strict);

			ChangeDataStateProperty(netSendable, NetSendableState.Serialized);

			//We setup the serializer so it returns an empty non-null array.
			encryptor.Setup(x => x.Encrypt(It.IsAny<byte[]>()))
				.Returns(new byte[0]);

			//act
			bool result = netSendable.Encrypt(encryptor.Object);

			//assert
			Assert.AreEqual(result, true);
			encryptor.Verify(x => x.Encrypt(It.IsAny<byte[]>()), Times.Once());
			Assert.AreEqual(netSendable.DataState, NetSendableState.Encrypted);
		}

		[Test]
		public static void Test_Decryption()
		{
			//arrange
			Mock<PacketPayload> payload = new Mock<PacketPayload>(MockBehavior.Strict);
			NetSendable<PacketPayload> netSendable = new NetSendable<PacketPayload>(payload.Object);
			Mock<IDecryptor> decryptor = new Mock<IDecryptor>(MockBehavior.Strict);

			ChangeDataStateProperty(netSendable, NetSendableState.Encrypted);

			//We use reflection to set the byte[]
			netSendable.GetType().GetField("byteData", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(netSendable, new byte[0]);

			//We setup the serializer so it returns an empty non-null array.
			decryptor.Setup(x => x.Decrypt(It.IsAny<byte[]>()))
				.Returns(new byte[0]);

			//act
			bool result = netSendable.Decrypt(decryptor.Object);

			//assert
			Assert.AreEqual(result, true);
			decryptor.Verify(x => x.Decrypt(It.IsAny<byte[]>()), Times.Once());
			Assert.AreEqual(netSendable.DataState, NetSendableState.Serialized);
		}
		#endregion

		#region State Invalid Operation Tests
		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		[TestCase(NetSendableState.Default)]
		[TestCase(NetSendableState.Encrypted)]
		public static void Test_Cant_Deserialize_In_States(NetSendableState state)
		{
			//arrange
			Mock<PacketPayload> payload = new Mock<PacketPayload>(MockBehavior.Strict);
			NetSendable<PacketPayload> netSendable = new NetSendable<PacketPayload>(payload.Object);

			ChangeDataStateProperty(netSendable, state);
			Assert.AreEqual(netSendable.DataState, state);

			//act
			netSendable.Deserialize(Mock.Of<IDeserializer>());

			//assert
			//should throw
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		[TestCase(NetSendableState.Serialized)]
		[TestCase(NetSendableState.Encrypted)]
		public static void Test_Cant_Serialize_In_States(NetSendableState state)
		{
			//arrange
			Mock<PacketPayload> payload = new Mock<PacketPayload>(MockBehavior.Strict);
			NetSendable<PacketPayload> netSendable = new NetSendable<PacketPayload>(payload.Object);

			ChangeDataStateProperty(netSendable, state);
			Assert.AreEqual(netSendable.DataState, state);

			//act
			netSendable.Serialize(Mock.Of<ISerializer>());

			//assert
			//should throw
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		[TestCase(NetSendableState.Serialized)]
		[TestCase(NetSendableState.Default)]
		public static void Test_Cant_Decrypt_In_States(NetSendableState state)
		{
			//arrange
			Mock<PacketPayload> payload = new Mock<PacketPayload>(MockBehavior.Strict);
			NetSendable<PacketPayload> netSendable = new NetSendable<PacketPayload>(payload.Object);

			ChangeDataStateProperty(netSendable, state);
			Assert.AreEqual(netSendable.DataState, state);

			//act
			netSendable.Decrypt(Mock.Of<IDecryptor>());

			//assert
			//should throw
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		[TestCase(NetSendableState.Encrypted)]
		[TestCase(NetSendableState.Default)]
		public static void Test_Cant_Encrypt_In_States(NetSendableState state)
		{
			//arrange
			Mock<PacketPayload> payload = new Mock<PacketPayload>(MockBehavior.Strict);
			NetSendable<PacketPayload> netSendable = new NetSendable<PacketPayload>(payload.Object);

			ChangeDataStateProperty(netSendable, state);
			Assert.AreEqual(netSendable.DataState, state);

			//act
			netSendable.Encrypt(Mock.Of<IEncryptor>());

			//assert
			//should throw
		}
		#endregion

		[Test]
		public static void Test_Decryption_With_Crypto_Throw()
		{
			//arrange
			NetSendable<PacketPayload> netSendable = new NetSendable<PacketPayload>(Mock.Of<PacketPayload>());
			//We need a throwing decryptor
			Mock<IDecryptor> decryptor = SetupThrowingDecryptor();
			//Change it to a serialized state so decryption is valid.
			ChangeDataStateProperty(netSendable, NetSendableState.Encrypted);
			bool result;

			//act
			result = netSendable.Decrypt(decryptor.Object);

			//assert
			//It should have failed so it should be false.
			Assert.IsFalse(result);
			Assert.AreEqual(netSendable.DataState, NetSendableState.Encrypted);
		}

		[Test]
		public static void Test_Encryption_With_Crypto_Throw()
		{
			//arrange
			NetSendable<PacketPayload> netSendable = new NetSendable<PacketPayload>(Mock.Of<PacketPayload>());
			//We need a throwing decryptor
			Mock<IEncryptor> encryptor = SetupThrowingIEncryptor();
			//Change it to a serialized state so decryption is valid.
			ChangeDataStateProperty(netSendable, NetSendableState.Serialized);
			bool result;

			//act
			result = netSendable.Encrypt(encryptor.Object);

			//assert
			//It should have failed so it should be false.
			Assert.IsFalse(result);
			Assert.AreEqual(netSendable.DataState, NetSendableState.Serialized);
		}

		[Test]
		public static void Test_Encryption_With_NullReturning_Encryptor()
		{
			//arrange
			NetSendable<PacketPayload> netSendable = new NetSendable<PacketPayload>(Mock.Of<PacketPayload>());
			//We need a throwing decryptor
			Mock<IEncryptor> encryptor = SetupNullReturningIEncryptor();
			//Change it to a serialized state so decryption is valid.
			ChangeDataStateProperty(netSendable, NetSendableState.Serialized);
			bool result;

			//act
			result = netSendable.Encrypt(encryptor.Object);

			//assert
			//It should have failed so it should be false.
			Assert.IsFalse(result);
			Assert.AreEqual(netSendable.DataState, NetSendableState.Serialized);
		}

		[Test]
		public static void Test_Decryption_With_NullReturning_Dencryptor()
		{
			//arrange
			NetSendable<PacketPayload> netSendable = new NetSendable<PacketPayload>(Mock.Of<PacketPayload>());
			//We need a throwing decryptor
			Mock<IDecryptor> decryptor = SetupNullReturningDecryptor();
			//Change it to a serialized state so decryption is valid.
			ChangeDataStateProperty(netSendable, NetSendableState.Encrypted);
			bool result;

			//act
			result = netSendable.Decrypt(decryptor.Object);

			//assert
			//It should have failed so it should be false.
			Assert.IsFalse(result);
			Assert.AreEqual(netSendable.DataState, NetSendableState.Encrypted);
		}

		[Test]
		public static void Test_Serialization_With_NullReturning_Serializer()
		{
			//arrange
			NetSendable<PacketPayload> netSendable = new NetSendable<PacketPayload>(Mock.Of<PacketPayload>());
			ISerializer serializer = SetupNullReturningSerializer().Object;
			bool result;

			//act
			result = netSendable.Serialize(serializer);

			//assert
			Assert.IsFalse(result);
			Assert.AreEqual(netSendable.DataState, NetSendableState.Default);
		}

		[Test]
		public static void Test_Deserialization_With_NullReturning_Deserializer()
		{
			//arrange
			NetSendable<PacketPayload> netSendable = new NetSendable<PacketPayload>(Mock.Of<PacketPayload>());
			IDeserializer deserializer = SetupNullReturningDeserializer().Object;
			bool result;
			ChangeDataStateProperty(netSendable, NetSendableState.Serialized);

			//act
			result = netSendable.Deserialize(deserializer);

			//assert
			Assert.IsFalse(result);
			Assert.AreEqual(netSendable.DataState, NetSendableState.Serialized);
		}


		private static void ChangeDataStateProperty<T>(NetSendable<T> netSendable, NetSendableState state)
			where T : class
		{
			netSendable.GetType().GetProperty("DataState").SetValue(netSendable, state);
		}

		[Test]
		public static void Test_ShallowCopy()
		{
			//arrange
			PacketPayload payload = Mock.Of<PacketPayload>();
			NetSendable<PacketPayload> sendable = new NetSendable<PacketPayload>(payload);

			//act
			NetSendable<PacketPayload> copiedSendableBeforeSerialization = sendable.ShallowClone();

			//A quick assert that the references on the payload are the same.
			Assert.AreEqual(copiedSendableBeforeSerialization.Data, sendable.Data);

			sendable.Serialize(Mock.Of<ISerializer>()); //We call this to change state
			NetSendable<PacketPayload> copiedSendable = sendable.ShallowClone();
			
			//assert
			Assert.AreEqual(copiedSendable.Data, sendable.Data);
			Assert.AreEqual(copiedSendable.DataState, sendable.DataState);
			Assert.AreNotEqual(copiedSendable.DataState, copiedSendableBeforeSerialization.DataState);
		}

		private static Mock<IDecryptor> SetupThrowingDecryptor()
		{
			Mock<IDecryptor> decryptor = new Mock<IDecryptor>(MockBehavior.Strict);
			decryptor.Setup(d => d.Decrypt(It.IsAny<byte[]>())).Throws<CryptographicException>();
			return decryptor;
		}

		private static Mock<IEncryptor> SetupThrowingIEncryptor()
		{
			Mock<IEncryptor> encryptor = new Mock<IEncryptor>(MockBehavior.Strict);
			encryptor.Setup(d => d.Encrypt(It.IsAny<byte[]>())).Throws<CryptographicException>();
			return encryptor;
		}

		private static Mock<IDecryptor> SetupNullReturningDecryptor()
		{
			Mock<IDecryptor> decryptor = new Mock<IDecryptor>(MockBehavior.Strict);
			decryptor.Setup(d => d.Decrypt(It.IsAny<byte[]>())).Returns(default(byte[]));
			return decryptor;
		}

		private static Mock<IEncryptor> SetupNullReturningIEncryptor()
		{
			Mock<IEncryptor> encryptor = new Mock<IEncryptor>(MockBehavior.Strict);
			encryptor.Setup(d => d.Encrypt(It.IsAny<byte[]>())).Returns(default(byte[]));
			return encryptor;
		}

		private static Mock<ISerializer> SetupNullReturningSerializer()
		{
			Mock<ISerializer> serializer = new Mock<ISerializer>(MockBehavior.Strict);
			serializer.Setup(d => d.Serialize<PacketPayload>(It.IsAny<PacketPayload>())).Returns(default(byte[]));
			return serializer;
		}

		private static Mock<IDeserializer> SetupNullReturningDeserializer()
		{
			Mock<IDeserializer> deserializer = new Mock<IDeserializer>(MockBehavior.Strict);
			deserializer.Setup(d => d.Deserialize<PacketPayload>(It.IsAny<byte[]>())).Returns(default(PacketPayload));
			return deserializer;
		}
	}
}
