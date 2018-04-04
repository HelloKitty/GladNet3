using System;
using System.Threading;
using System.Threading.Tasks;
using FreecraftCore.Serializer;
using GladNet;
using JetBrains.Annotations;

namespace GladNet
{
	public sealed class AsyncWireReaderBytesReadableAdapter : IWireStreamReaderStrategyAsync
	{
		private IBytesReadable BytesReadableSource { get; }

		/// <inheritdoc />
		public AsyncWireReaderBytesReadableAdapter([NotNull] IBytesReadable bytesReadableSource)
		{
			if(bytesReadableSource == null) throw new ArgumentNullException(nameof(bytesReadableSource));

			BytesReadableSource = bytesReadableSource;
		}

		/// <inheritdoc />
		public void Dispose()
		{
			//Do nothing
		}

		/// <inheritdoc />
		public byte ReadByte()
		{
			return BytesReadableSource.Read(1)[0];
		}

		/// <inheritdoc />
		public byte PeekByte()
		{
			throw new NotSupportedException("Peeking is not supported on a byte readable source directly.");
		}

		/// <inheritdoc />
		public byte[] ReadAllBytes()
		{
			throw new NotSupportedException("Reading all from an infinite byte readable source is not not supported.");
		}

		/// <inheritdoc />
		public byte[] ReadBytes(int count)
		{
			return BytesReadableSource.Read(count);
		}

		/// <inheritdoc />
		public byte[] PeekBytes(int count)
		{
			throw new NotSupportedException("Peeking is not supported on a byte readable source directly.");
		}

		//TODO: make this more efficient
		/// <inheritdoc />
		public async Task<byte> ReadByteAsync()
		{
			//TODO: This will fail when the socket disconnects
			return (await ReadBytesAsync(1)
				.ConfigureAwait(false))[0];
		}

		/// <inheritdoc />
		public Task<byte> PeekByteAsync()
		{
			throw new NotSupportedException("Peeking is not supported on a byte readable source directly.");
		}

		/// <inheritdoc />
		public Task<byte[]> ReadAllBytesAsync()
		{
			throw new NotSupportedException("Reading all from an infinite byte readable source is not not supported.");
		}

		/// <inheritdoc />
		public async Task<byte[]> ReadBytesAsync(int count)
		{
			byte[] bytes = new byte[count];

			int i = await BytesReadableSource.ReadAsync(bytes, 0, count, CancellationToken.None)
				.ConfigureAwait(false);

			//0 means the socket disconnected
			if(i == 0)
				return null;

			if(i != count)
				throw new InvalidOperationException($"Failed to read {count} many bytes from {nameof(IBytesReadable)}. Read: {i}");

			return bytes;
		}

		/// <inheritdoc />
		public Task<byte[]> PeekBytesAsync(int count)
		{
			throw new NotSupportedException("Peeking is not supported on a byte readable source directly.");
		}
	}
}