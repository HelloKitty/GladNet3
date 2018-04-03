using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace GladNet
{
	public sealed class NetworkClientBufferWriteUntilSizeReachedDecorator : NetworkClientBase, IConnectable, IDisconnectable, IDisposable,
		IBytesWrittable, IBytesReadable
	{
		/// <summary>
		/// The decorated network client.
		/// </summary>
		private NetworkClientBase DecoratedClient { get; }

		/// <summary>
		/// The current index of the buffer.
		/// </summary>
		private int CurrentIndex { get; set; } = -1;

		private AsyncLockableBuffer BufferedData { get; }

		private AsyncLockableBuffer CombinedBuffer { get; }

		public NetworkClientBufferWriteUntilSizeReachedDecorator(NetworkClientBase decoratedClient, int bufferedWaitSize, int bufferedCombinedSize = 30000)
		{
			if(decoratedClient == null) throw new ArgumentNullException(nameof(decoratedClient));
			if(bufferedWaitSize <= 1) throw new ArgumentOutOfRangeException(nameof(bufferedWaitSize), "Do not use this decorator if you don't need buffered data.");

			DecoratedClient = decoratedClient;
			BufferedData = new AsyncLockableBuffer(bufferedWaitSize);
			CombinedBuffer = new AsyncLockableBuffer(bufferedCombinedSize);
		}

		/// <inheritdoc />
		public override Task<int> ReadAsync(byte[] buffer, int start, int count, CancellationToken token)
		{
			return DecoratedClient.ReadAsync(buffer, start, count, token)
;
		}

		/// <inheritdoc />
		public override Task ClearReadBuffers()
		{
			return DecoratedClient.ClearReadBuffers();
		}

		/// <inheritdoc />
		public override Task DisconnectAsync(int delay)
		{
			return DecoratedClient.DisconnectAsync(delay);
		}

		/// <inheritdoc />
		public override async Task WriteAsync(byte[] bytes, int offset, int count)
		{
			if(count < 0) throw new ArgumentOutOfRangeException(nameof(count));

			//If we have more bytes than we require buffering till
			//we should just write
			//We have to lock to prevent anything from touching the combined or buffer inbetween
			using(await BufferedData.BufferLock.LockAsync().ConfigureAwait(false))
			using(await CombinedBuffer.BufferLock.LockAsync().ConfigureAwait(false))
			{
				if(count > BufferedData.Buffer.Length && CurrentIndex == -1)
				{
					await DecoratedClient.WriteAsync(bytes, offset, count)
						.ConfigureAwait(false);
				}
				else if(count + CurrentIndex + 1 > BufferedData.Buffer.Length && CurrentIndex != -1)
				{
					//TODO: Do this somehow without copying
					BufferUtil.QuickUnsafeCopy(BufferedData.Buffer, 0, CombinedBuffer.Buffer, 0, CurrentIndex + 1);
					BufferUtil.QuickUnsafeCopy(bytes, offset, CombinedBuffer.Buffer, CurrentIndex + 1, count);

					await DecoratedClient.WriteAsync(CombinedBuffer.Buffer, 0, count + CurrentIndex + 1)
						.ConfigureAwait(false);
					CurrentIndex = -1;
				}
				else
				{
					//At this point we know that the buffer isn't large enough to write so we need to buffer it
					BufferUtil.QuickUnsafeCopy(bytes, offset, BufferedData.Buffer, CurrentIndex + 1, count);
					CurrentIndex += count;
				}
			}
		}

		/// <inheritdoc />
		public override Task<bool> ConnectAsync(string ip, int port)
		{
			return DecoratedClient.ConnectAsync(ip, port)
;
		}
	}
}
