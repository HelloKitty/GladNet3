using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace GladNet
{
	/// <summary>
	/// Crypto decroator for the <see cref="NetworkClientBase"/> that extends the <see cref="ReadAsync"/> and
	/// <see cref="WriteAsync"/> methods with an encryption implementation. It will use a fixed
	/// blocksize for reading and writing data.
	/// </summary>
	public sealed class NetworkClientFixedBlockSizeCryptoDecorator : NetworkClientBase, IConnectable, IDisconnectable, IDisposable,
		IBytesWrittable, IBytesReadable
	{
		/// <summary>
		/// The decorated <see cref="NetworkClientBase"/>.
		/// </summary>
		private NetworkClientBase DecoratedClient { get; }

		/// <summary>
		/// The encryption service provider for the connection.
		/// </summary>
		private ICryptoServiceProvider EncryptionServiceProvider { get; }

		/// <summary>
		/// The decryption service provider for the connection.
		/// </summary>
		private ICryptoServiceProvider DecryptionServiceProvider { get; }

		/// <summary>
		/// Indicates the size of the block for crypto padding.
		/// </summary>
		private int BlockSize { get; }

		/// <summary>
		/// This is a buffer that can hold the overflow for som,e
		/// </summary>
		private byte[] CryptoBlockOverflow { get; }

		private int CryptoBlockOverflowReadIndex { get; set; }

		/// <summary>
		/// Lockable write buffer.
		/// </summary>
		private AsyncLockableBuffer WriteBuffer { get; }

		/// <summary>
		/// Lockable read buffer.
		/// </summary>
		private AsyncLockableBuffer ReadBuffer { get; }

		/// <summary>
		/// Creates a new crypto decorator for the <see cref="NetworkClientBase"/>.
		/// Extends the <see cref="ReadAsync"/> and <see cref="WriteAsync"/> implementations to pass
		/// all bytes through the corresponding incoming and outgoing ciphers.
		/// </summary>
		/// <param name="decoratedClient">The client to decorate.</param>
		/// <param name="encryptionServiceProvider">The encryption service.</param>
		/// <param name="decryptionServiceProvider">The decryption service.</param>
		public NetworkClientFixedBlockSizeCryptoDecorator(NetworkClientBase decoratedClient, ICryptoServiceProvider encryptionServiceProvider, ICryptoServiceProvider decryptionServiceProvider, int blockSize, int cryptoBufferSize = 30000)
		{
			if(decoratedClient == null) throw new ArgumentNullException(nameof(decoratedClient));
			if(encryptionServiceProvider == null) throw new ArgumentNullException(nameof(encryptionServiceProvider));
			if(decryptionServiceProvider == null) throw new ArgumentNullException(nameof(decryptionServiceProvider));
			if(blockSize <= 0) throw new ArgumentOutOfRangeException(nameof(blockSize));

			DecoratedClient = decoratedClient;
			EncryptionServiceProvider = encryptionServiceProvider;
			DecryptionServiceProvider = decryptionServiceProvider;
			BlockSize = blockSize;

			//One of the lobby packets is 14,000 bytes. We may even need bigger.
			ReadBuffer = new AsyncLockableBuffer(cryptoBufferSize);
			WriteBuffer = new AsyncLockableBuffer(cryptoBufferSize);

			CryptoBlockOverflow = new byte[Math.Max(blockSize - 1, 0)]; //we only need
			CryptoBlockOverflowReadIndex = CryptoBlockOverflow.Length; //set this to last index to indicate empty initially
		}

		public override async Task<bool> ConnectAsync(string ip, int port)
		{
			return await DecoratedClient.ConnectAsync(ip, port)
				.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public override async Task ClearReadBuffers()
		{
			using(await ReadBuffer.BufferLock.LockAsync().ConfigureAwait(false))
			{
				//Reset the crypto buffer
				CryptoBlockOverflowReadIndex = CryptoBlockOverflow.Length;
				await DecoratedClient.ClearReadBuffers()
					.ConfigureAwait(false);
			}
		}

		/// <inheritdoc />
		public override async Task DisconnectAsync(int delay)
		{
			await DecoratedClient.DisconnectAsync(delay)
				.ConfigureAwait(false);
		}

		//TODO: Refactor this
		/// <summary>
		/// Reads asyncronously <see cref="count"/> many bytes from the reader.
		/// </summary>
		/// <param name="buffer">The buffer to store the bytes into.</param>
		/// <param name="start">The start position in the buffer to start reading into.</param>
		/// <param name="count">How many bytes to read.</param>
		/// <param name="token">The cancel token to check during the async operation.</param>
		/// <returns>A future for the read bytes.</returns>
		public override async Task<int> ReadAsync(byte[] buffer, int start, int count, CancellationToken token)
		{
			if(buffer == null) throw new ArgumentNullException(nameof(buffer));
			if(start < 0) throw new ArgumentOutOfRangeException(nameof(start));
			if(count < 0) throw new ArgumentOutOfRangeException(nameof(count), $"Cannot read less than 0 bytes. Can't read: {count} many bytes");

			//We read this outside of the lock to reduce locking time
			//If the above caller requested an invalid count of bytes to read
			//We should try to correct for it and read afew more bytes.
			int extendedCount = ConvertToBlocksizeCount(count);

			if(token.IsCancellationRequested)
				return 0;

			//TODO: Optimize for when the buffer is large enough. Right now it does needless BlockCopy even if there is room in the buffer
			//We should lock incase there are multiple calls
			using(await ReadBuffer.BufferLock.LockAsync(token).ConfigureAwait(false))
			{
				int cryptoOverflowSize = CryptoBlockOverflow.Length - CryptoBlockOverflowReadIndex;

				//If the overflow size is the exact size asked for then
				//we don't need to do anything but copy the buffer and return
				if(cryptoOverflowSize >= count)
				{
					//Read from the crypto overflow and move the read index forward
					BufferUtil.QuickUnsafeCopy(CryptoBlockOverflow, CryptoBlockOverflowReadIndex, buffer, start, count);
					CryptoBlockOverflowReadIndex = CryptoBlockOverflowReadIndex + count;
					return count;
				}
				else if(cryptoOverflowSize == 0)
				{
					//Since the buffer MAY be large enough we check to avoid a potential BlockCopy
					if(buffer.Length > extendedCount)
					{
						//Since the provider may not give a buffer large enough we should use the crypto buffer
						bool result = await ReadAndDecrypt(buffer, start, token, extendedCount)
							.ConfigureAwait(false);

						//if the read/decrypt failed then return 0
						if(!result)
							return 0;

						FillCryptoOverflowWithExcess(buffer, 0, count, extendedCount);
					}
					else
					{
						//Since the provider may not give a buffer large enough we should use the crypto buffer
						bool result = await ReadAndDecrypt(ReadBuffer.Buffer, 0, token, extendedCount)
							.ConfigureAwait(false);

						//Now we must BlockCopy the requested amount into the buffer
						BufferUtil.QuickUnsafeCopy(ReadBuffer.Buffer, 0, buffer, start, count);

						//if the read/decrypt failed then return 0
						if(!result)
							return 0;

						FillCryptoOverflowWithExcess(ReadBuffer.Buffer, 0, count, extendedCount);
					}
				}
				else if(cryptoOverflowSize < count)
				{
					BufferUtil.QuickUnsafeCopy(CryptoBlockOverflow, CryptoBlockOverflowReadIndex, buffer, start, cryptoOverflowSize);
					CryptoBlockOverflowReadIndex = CryptoBlockOverflow.Length; //set it to last index so we know we have no overflow anymore

					//Recompute the extended count, since we read some of the cryptooverflow it will have changed potentially
					int newCount = count - cryptoOverflowSize;
					extendedCount = ConvertToBlocksizeCount(newCount);

					//We can't directly read this into the buffer provided
					//it may not be large enough to handle the blocksize shift
					//So we must read with the cryptobuffer and then blockcopy the result
					if(buffer.Length > extendedCount + start + cryptoOverflowSize)
					{
						//Read into buffer starting at the start requested + the overflow size
						bool result = await ReadAndDecrypt(buffer, start + cryptoOverflowSize, token, extendedCount)
							.ConfigureAwait(false);

						//if the read/decrypt failed then return 0
						if(!result)
							return 0;

						FillCryptoOverflowWithExcess(buffer, start + cryptoOverflowSize, newCount, extendedCount);
					}
					else
					{
						//Read into buffer starting at the start requested + the overflow size
						bool result = await ReadAndDecrypt(ReadBuffer.Buffer, 0, token, extendedCount)
							.ConfigureAwait(false);

						//if the read/decrypt failed then return 0
						if(!result)
							return 0;

						//Now we must BlockCopy the requested amount into the buffer
						BufferUtil.QuickUnsafeCopy(ReadBuffer.Buffer, 0, buffer, start + cryptoOverflowSize, newCount);

						FillCryptoOverflowWithExcess(ReadBuffer.Buffer, 0, newCount, extendedCount);
					}
				}
			}

			//never return the extended count, callers above shouldn't have to deal
			//with more than they asked for
			return count;
		}

		private void FillCryptoOverflowWithExcess(byte[] buffer, int start, int initialCount, int extendedCount)
		{
			//If the original count asked for is not the same as the blocksize adjusted count
			//we need to buffer the extra bytes
			if(initialCount != extendedCount)
			{
				int overflowSize = extendedCount - initialCount;
				int readIndex = CryptoBlockOverflow.Length - overflowSize;

				//We should read the overflow into starting index OverflowLength - overflowSize index and not starting at 0
				//This allows us to know how many and what index to starting reading at all at the same time
				BufferUtil.QuickUnsafeCopy(buffer, start + (extendedCount - overflowSize), CryptoBlockOverflow, readIndex, overflowSize);
				CryptoBlockOverflowReadIndex = readIndex;
			}
		}

		private async Task<bool> ReadAndDecrypt(byte[] buffer, int start, CancellationToken token, int extendedCount)
		{
			//We don't need to know the amount read, I think.
			await DecoratedClient.ReadAsync(buffer, start, extendedCount, token)
				.ConfigureAwait(false);

			//Check cancel again, we want to fail quick
			if(token.IsCancellationRequested)
				return false;

			//We throw above if we have an invalid size that can't be decrypted once read.
			//That means callers will need to be careful in what they request to read.
			DecryptionServiceProvider.Crypt(buffer, start, extendedCount);

			//Check cancel again, we want to fail quick
			if(token.IsCancellationRequested)
				return false;

			return true;
		}

		public override async Task WriteAsync(byte[] bytes, int offset, int count)
		{
			if(offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
			if(count < 0) throw new ArgumentOutOfRangeException(nameof(count));

			int blocksizeAdjustedCount = ConvertToBlocksizeCount(count);

			if(count == blocksizeAdjustedCount)
				await DecoratedClient.WriteAsync(EncryptionServiceProvider.Crypt(bytes, offset, count), offset, count)
					.ConfigureAwait(false);
			else
			{
				//Lock because we use the crypto buffer for this
				using(await WriteBuffer.BufferLock.LockAsync().ConfigureAwait(false))
				{
					try
					{
						//We copy to the thread local buffer so we can use it as an extended buffer by "neededBytes" many more bytes.
						//So the buffer is very large but we'll tell it to write bytes.length + neededBytes.
						BufferUtil.QuickUnsafeCopy(bytes, offset, WriteBuffer.Buffer, 0, count); //dont copy full array, might only need less with count
					}
					catch(Exception e)
					{
						throw new InvalidOperationException($"Failed to copy bytes to crypto buffer. Bytes Length: {bytes.Length} Offset: {offset} Count: {count} BlocksizeAdjustedCount: {blocksizeAdjustedCount}", e);
					}

					EncryptionServiceProvider.Crypt(WriteBuffer.Buffer, 0, blocksizeAdjustedCount);

					//recurr to write the bytes with the now properly sized buffer.
					await DecoratedClient.WriteAsync(WriteBuffer.Buffer, 0, blocksizeAdjustedCount)
						.ConfigureAwait(false);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int ConvertToBlocksizeCount(int count)
		{
			int remainder = count % BlockSize;

			//Important to check if it's already perfectly size
			//otherwise below code will return count + blocksize
			if(remainder == 0)
				return count;

			return count + (BlockSize - (count % BlockSize));
		}
	}
}
