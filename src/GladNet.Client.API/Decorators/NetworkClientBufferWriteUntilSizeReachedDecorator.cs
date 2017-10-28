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

		/// <summary>
		/// Buffer that will temporarily hold the data if it's
		/// smaller than the buffered size.
		/// </summary>
		private byte[] BufferedData { get; }

		/// <summary>
		/// Buffer to be copied into from the buffered data and the newly written data.
		/// </summary>
		private byte[] BufferCombinedBuffer { get; }

		private readonly AsyncLock writeSyncObj = new AsyncLock();

		public NetworkClientBufferWriteUntilSizeReachedDecorator(NetworkClientBase decoratedClient, int bufferedWaitSize, int bufferedCombinedSize = 30000)
		{
			if(decoratedClient == null) throw new ArgumentNullException(nameof(decoratedClient));
			if(bufferedWaitSize <= 1) throw new ArgumentOutOfRangeException(nameof(bufferedWaitSize), "Do not use this decorator if you don't need buffered data.");

			DecoratedClient = decoratedClient;
			BufferedData = new byte[bufferedWaitSize];
			BufferCombinedBuffer = new byte[bufferedCombinedSize];
		}

		/// <inheritdoc />
		public override async Task<int> ReadAsync(byte[] buffer, int start, int count, CancellationToken token)
		{
			return await DecoratedClient.ReadAsync(buffer, start, count, token);
		}

		/// <inheritdoc />
		public override async Task DisconnectAsync(int delay)
		{
			await DecoratedClient.DisconnectAsync(delay);
		}

		/// <inheritdoc />
		public override async Task WriteAsync(byte[] bytes, int offset, int count)
		{
			if(count < 0) throw new ArgumentOutOfRangeException(nameof(count));

			//If we have more bytes than we require buffering till
			//we should just write
			//We have to lock to prevent anything from touching the combined or buffer inbetween
			using(await writeSyncObj.LockAsync())
			{
				if(count > BufferedData.Length && CurrentIndex == -1)
				{
					await DecoratedClient.WriteAsync(bytes, offset, count);
				}
				else if(count + CurrentIndex + 1 > BufferedData.Length && CurrentIndex != -1)
				{
					//TODO: Do this somehow without copying
					Buffer.BlockCopy(BufferedData, 0, BufferCombinedBuffer, 0, CurrentIndex + 1);
					Buffer.BlockCopy(bytes, offset, BufferCombinedBuffer, CurrentIndex + 1, count);

					await DecoratedClient.WriteAsync(BufferCombinedBuffer, 0, count + CurrentIndex + 1);
					CurrentIndex = -1;
				}
				else
				{
					//At this point we know that the buffer isn't large enough to write so we need to buffer it
					Buffer.BlockCopy(bytes, offset, BufferedData, CurrentIndex + 1, count);
					CurrentIndex += count;
				}
			}
		}

		/// <inheritdoc />
		public override async Task<bool> ConnectAsync(string ip, int port)
		{
			return await DecoratedClient.ConnectAsync(ip, port);
		}
	}
}
