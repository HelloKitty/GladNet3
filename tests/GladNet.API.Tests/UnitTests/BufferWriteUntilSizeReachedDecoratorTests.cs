using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace GladNet.API.Tests.UnitTests
{
	[TestFixture]
	public class BufferWriteUntilSizeReachedDecoratorTests
	{
		[Test]
		[TestCase(4, 1)]
		[TestCase(4, 2)]
		[TestCase(4, 3)]
		[TestCase(4, 4)]
		[TestCase(4, 5)]
		[TestCase(7, 1)]
		[TestCase(7, 2)]
		[TestCase(7, 3)]
		[TestCase(7, 4)]
		[TestCase(7, 5)]
		[TestCase(7, 13)]
		[TestCase(7, 14)]
		public void Test_Can_Write_EquallySizedChunks(int waitSize, int bufferWriteSize)
		{
			//arrange
			Mock<NetworkClientBase> mockedClient = new Mock<NetworkClientBase>();
			byte[] buffer = new byte[200];
			Nullable<int> trackedOffset = 0;

			mockedClient.Setup(o => o.WriteAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
				.Callback((byte[] bytes, int offset, int count) =>
				{
					if(count < 0) throw new ArgumentOutOfRangeException(nameof(count));

					Buffer.BlockCopy(bytes, offset, buffer, trackedOffset.Value, count);
					trackedOffset += count;
				})
				.Returns(() => Task.CompletedTask);

			NetworkClientBufferWriteUntilSizeReachedDecorator decorator = new NetworkClientBufferWriteUntilSizeReachedDecorator(mockedClient.Object, waitSize);

			//act
			for(int i = 0; i < (buffer.Length / bufferWriteSize) * bufferWriteSize; i += bufferWriteSize)
			{
				byte[] bufferToWrite = Enumerable.Range(trackedOffset.Value, bufferWriteSize)
					.Select(val => (byte)val)
					.ToArray();

				decorator.WriteAsync(bufferToWrite, 0, bufferWriteSize).Wait();
			}

			//assert
			for(int i = 0; i < buffer.Length && (buffer[i] != 0 && i != 0); i++)
			{
				Assert.AreEqual(i, buffer[i]);
			}
		}
	}
}
