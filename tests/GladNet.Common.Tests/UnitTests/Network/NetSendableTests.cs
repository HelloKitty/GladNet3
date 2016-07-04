using GladNet.Payload;
using GladNet.Serializer;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common.Tests
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
		public static void Test_Construction_Null_TData()
		{
			Assert.Throws<ArgumentNullException>(() => new NetSendable<PacketPayload>(null));
		}

		[Test]
		[TestCase("Serialize")]
		[TestCase("Deserialize")]
		[TestCase("Decrypt")]
		[TestCase("Encrypt")]
		public static void Test_Methods_With_Null(string methodName)
		{
			//arrange
			Mock<PacketPayload> payload = new Mock<PacketPayload>(MockBehavior.Strict);
			NetSendable<PacketPayload> netSendable = new NetSendable<PacketPayload>(payload.Object);

			//assert
			//Should throw an exception.
			//We check inner exception because it gets consumed by a reflection exception.
			//We should also rely on Assert and not ExpectedException because it's possible that something else
			//could be throwing if we're not explictly checking what we care about.
			Assert.That(() => netSendable.GetType().GetMethod(methodName).Invoke(netSendable, new object[1] { null }),
				Throws.InnerException.TypeOf<ArgumentNullException>());
		}

		#region State Valid Operation Tests
		[Test]
		public static void Test_Serialization()
		{
			//arrange
			Mock<PacketPayload> payload = new Mock<PacketPayload>(MockBehavior.Strict);
			NetSendable<PacketPayload> netSendable = new NetSendable<PacketPayload>(payload.Object);
			Mock<ISerializerStrategy> serializer = new Mock<ISerializerStrategy>(MockBehavior.Strict);

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
			Mock<IDeserializerStrategy> deserializer = new Mock<IDeserializerStrategy>(MockBehavior.Strict);

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
			Mock<IEncryptorStrategy> encryptor = new Mock<IEncryptorStrategy>(MockBehavior.Strict);

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
			Mock<IDecryptorStrategy> decryptor = new Mock<IDecryptorStrategy>(MockBehavior.Strict);

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
		public static void Test_Cant_Deserialize_In_States([EnumRange(typeof(NetSendableState), NetSendableState.Serialized)] NetSendableState state)
		{
			//arrange
			Mock<PacketPayload> payload = new Mock<PacketPayload>(MockBehavior.Strict);
			NetSendable<PacketPayload> netSendable = new NetSendable<PacketPayload>(payload.Object);

			//act (set the state for testing
			ChangeDataStateProperty(netSendable, state);

			//assert
			//Check explictly. Something else could have thrown.
			Assert.That(() => netSendable.Deserialize(Mock.Of<IDeserializerStrategy>()), Throws.Exception.TypeOf<InvalidOperationException>());
		}

		[Test]
		public static void Test_Cant_Serialize_In_States([EnumRange(typeof(NetSendableState), NetSendableState.Default)] NetSendableState state)
		{
			//arrange
			Mock<PacketPayload> payload = new Mock<PacketPayload>(MockBehavior.Strict);
			NetSendable<PacketPayload> netSendable = new NetSendable<PacketPayload>(payload.Object);

			//act
			ChangeDataStateProperty(netSendable, state);
	
			//Assert
			Assert.That(() => netSendable.Serialize(Mock.Of<ISerializerStrategy>()), Throws.Exception.TypeOf<InvalidOperationException>());
		}

		[Test]
		public static void Test_Cant_Decrypt_In_States([EnumRange(typeof(NetSendableState), NetSendableState.Encrypted)] NetSendableState state)
		{
			//arrange
			Mock<PacketPayload> payload = new Mock<PacketPayload>(MockBehavior.Strict);
			NetSendable<PacketPayload> netSendable = new NetSendable<PacketPayload>(payload.Object);

			ChangeDataStateProperty(netSendable, state);

			//assert
			//should throw
			Assert.Throws<InvalidOperationException>(() => netSendable.Decrypt(Mock.Of<IDecryptorStrategy>()));
		}

		[Test]
		public static void Test_Cant_Encrypt_In_States([EnumRange(typeof(NetSendableState), NetSendableState.Serialized)] NetSendableState state)
		{
			//arrange
			Mock<PacketPayload> payload = new Mock<PacketPayload>(MockBehavior.Strict);
			NetSendable<PacketPayload> netSendable = new NetSendable<PacketPayload>(payload.Object);

			//act
			ChangeDataStateProperty(netSendable, state);

			//assert
			Assert.That(() => netSendable.Encrypt(Mock.Of<IEncryptorStrategy>()), Throws.Exception.TypeOf<InvalidOperationException>());
		}
		#endregion

		[Test]
		public static void Test_Decryption_With_Crypto_Throw()
		{
			//arrange
			NetSendable<PacketPayload> netSendable = new NetSendable<PacketPayload>(Mock.Of<PacketPayload>());
			//We need a throwing decryptor
			Mock<IDecryptorStrategy> decryptor = SetupThrowingDecryptor();
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
			Mock<IEncryptorStrategy> encryptor = SetupThrowingIEncryptor();
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
			Mock<IEncryptorStrategy> encryptor = SetupNullReturningIEncryptor();
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
			Mock<IDecryptorStrategy> decryptor = SetupNullReturningDecryptor();
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
			ISerializerStrategy serializer = SetupNullReturningSerializer().Object;
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
			IDeserializerStrategy deserializer = SetupNullReturningDeserializer().Object;
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

			Assert.AreEqual(netSendable.DataState, state);
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

			sendable.Serialize(Mock.Of<ISerializerStrategy>()); //We call this to change state
			NetSendable<PacketPayload> copiedSendable = sendable.ShallowClone();
			
			//assert
			Assert.AreEqual(copiedSendable.Data, sendable.Data);
			Assert.AreEqual(copiedSendable.DataState, sendable.DataState);
			Assert.AreNotEqual(copiedSendable.DataState, copiedSendableBeforeSerialization.DataState);
		}

		private static Mock<IDecryptorStrategy> SetupThrowingDecryptor()
		{
			Mock<IDecryptorStrategy> decryptor = new Mock<IDecryptorStrategy>(MockBehavior.Strict);
			decryptor.Setup(d => d.Decrypt(It.IsAny<byte[]>())).Throws<CryptographicException>();
			return decryptor;
		}

		private static Mock<IEncryptorStrategy> SetupThrowingIEncryptor()
		{
			Mock<IEncryptorStrategy> encryptor = new Mock<IEncryptorStrategy>(MockBehavior.Strict);
			encryptor.Setup(d => d.Encrypt(It.IsAny<byte[]>())).Throws<CryptographicException>();
			return encryptor;
		}

		private static Mock<IDecryptorStrategy> SetupNullReturningDecryptor()
		{
			Mock<IDecryptorStrategy> decryptor = new Mock<IDecryptorStrategy>(MockBehavior.Strict);
			decryptor.Setup(d => d.Decrypt(It.IsAny<byte[]>())).Returns(default(byte[]));
			return decryptor;
		}

		private static Mock<IEncryptorStrategy> SetupNullReturningIEncryptor()
		{
			Mock<IEncryptorStrategy> encryptor = new Mock<IEncryptorStrategy>(MockBehavior.Strict);
			encryptor.Setup(d => d.Encrypt(It.IsAny<byte[]>())).Returns(default(byte[]));
			return encryptor;
		}

		private static Mock<ISerializerStrategy> SetupNullReturningSerializer()
		{
			Mock<ISerializerStrategy> serializer = new Mock<ISerializerStrategy>(MockBehavior.Strict);
			serializer.Setup(d => d.Serialize<PacketPayload>(It.IsAny<PacketPayload>())).Returns(default(byte[]));
			return serializer;
		}

		private static Mock<IDeserializerStrategy> SetupNullReturningDeserializer()
		{
			Mock<IDeserializerStrategy> deserializer = new Mock<IDeserializerStrategy>(MockBehavior.Strict);
			deserializer.Setup(d => d.Deserialize<PacketPayload>(It.IsAny<byte[]>())).Returns(default(PacketPayload));
			return deserializer;
		}
	}
}
