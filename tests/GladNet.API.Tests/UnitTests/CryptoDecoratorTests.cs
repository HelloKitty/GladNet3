using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace GladNet.API.Tests
{
	[TestFixture]
	public class CryptoDecoratorTests
	{
		[Test]
		public void Test_Can_Construct_Crypto_Decorator_WithNoBufferSize()
		{
			//arrange
			NetworkClientBase mockedClient = Mock.Of<NetworkClientBase>();
			ICryptoServiceProvider encryptProvider = Mock.Of<ICryptoServiceProvider>();
			ICryptoServiceProvider decryptProvider = Mock.Of<ICryptoServiceProvider>();

			NetworkClientFixedBlockSizeCryptoDecorator decorator = new NetworkClientFixedBlockSizeCryptoDecorator(mockedClient, encryptProvider, decryptProvider, 1);
		}

		[Test]
		[TestCase(1, 4)]
		[TestCase(2, 4)]
		[TestCase(3, 4)]
		[TestCase(4, 4)]
		[TestCase(5, 4)]
		[TestCase(6, 4)]
		[TestCase(7, 4)]
		[TestCase(8, 4)]
		[TestCase(9, 4)]
		public void Test_Can_Read_Correct_EqualSizeChunks(int blockSize, int readSize)
		{
			//arrange
			NetworkClientBase decorator = SetupNetworkClient(readSize, blockSize);

			byte[] readBuffer = new byte[readSize];

			//assert
			for(int i = 0; i < 100; i = i + 4)
			{
				int result = decorator.ReadAsync(readBuffer, 0, readSize, CancellationToken.None).Result;

				Assert.AreEqual(readSize, result, "Crypto reader didn't read the requested amount of bytes.");
				
				for(int j = 0; j < readBuffer.Length; j++)
					Assert.AreEqual((i + j) * 2, readBuffer[j], "Value read from crypto read wasn't the expected value.");
			}
		}

		[Test]
		[TestCase(1, 4)]
		[TestCase(2, 4)]
		[TestCase(3, 4)]
		[TestCase(4, 4)]
		[TestCase(5, 4)]
		[TestCase(6, 4)]
		[TestCase(7, 4)]
		[TestCase(8, 4)]
		[TestCase(9, 4)]
		public void Test_Can_Read_Correct_EqualSizeChunks_With_Large_Buffer_Size(int blockSize, int readSize)
		{
			//arrange
			NetworkClientBase decorator = SetupNetworkClient(readSize, blockSize);

			byte[] readBuffer = new byte[readSize * 500];

			//assert
			for(int i = 0; i < 100; i = i + 4)
			{
				int result = decorator.ReadAsync(readBuffer, 0, readSize, CancellationToken.None).Result;

				Assert.AreEqual(readSize, result, "Crypto reader didn't read the requested amount of bytes.");

				for(int j = 0; j < readSize; j++)
					Assert.AreEqual((i + j) * 2, readBuffer[j], "Value read from crypto read wasn't the expected value.");
			}
		}

		private NetworkClientBase SetupNetworkClient(int readSize, int blockSize)
		{
			Mock<NetworkClientBase> mockedClient = new Mock<NetworkClientBase>();
			ICryptoServiceProvider encryptProvider = Mock.Of<ICryptoServiceProvider>();
			Mock<ICryptoServiceProvider> decryptProvider = new Mock<ICryptoServiceProvider>();

			int[] intRef = new int[1];

			//Setup the client to produce incrementing values in the byte array
			mockedClient.Setup(o => o.ReadAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
				.Returns((byte[] buffer, int start, int count, CancellationToken token) =>
				{
					for(int i = 0; i < count; i++)
					{
						buffer[i + start] = (byte)intRef[0];
						intRef[0] = intRef[0] + 1;
					}

					return Task.FromResult(readSize);
				});

			decryptProvider.Setup(o => o.Crypt(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns((byte[] bytes, int offset, int count) =>
				{
					for(int i = 0; i < count; i++)
						bytes[i + offset] = (byte)(bytes[i + offset] * 2);

					return bytes;
				});

			return new NetworkClientFixedBlockSizeCryptoDecorator(mockedClient.Object, encryptProvider, decryptProvider.Object, blockSize);
		}
	}
}
