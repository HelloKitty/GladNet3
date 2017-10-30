using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace GladNet
{
	/// <summary>
	/// Decorates the <see cref="INetworkMessageClient{TReadPayloadType,TWritePayloadType}"/> with buffer clearing functionality
	/// after a message has been read.
	/// </summary>
	/// <typeparam name="TClientType"></typeparam>
	/// <typeparam name="TReadPayloadBaseType"></typeparam>
	/// <typeparam name="TWritePayloadBaseType"></typeparam>
	/// <typeparam name="TPayloadConstraintType"></typeparam>
	public sealed class NetworkClientNetworkMessageReadingClearBuffersAfterReadDecorator<TClientType, TReadPayloadBaseType, TWritePayloadBaseType> : NetworkClientBase,
		INetworkMessageClient<TReadPayloadBaseType, TWritePayloadBaseType>
		where TClientType : NetworkClientBase, INetworkMessageClient<TReadPayloadBaseType, TWritePayloadBaseType>
		where TReadPayloadBaseType : class
		where TWritePayloadBaseType : class
	{
		/// <summary>
		/// The decorated client.
		/// </summary>
		private TClientType DecoratedClient { get; }

		private AsyncLock ReadLock { get; } = new AsyncLock();

		public NetworkClientNetworkMessageReadingClearBuffersAfterReadDecorator(TClientType decoratedClient)
		{
			if(decoratedClient == null) throw new ArgumentNullException(nameof(decoratedClient));

			DecoratedClient = decoratedClient;
		}

		/// <inheritdoc />
		public override async Task<int> ReadAsync(byte[] buffer, int start, int count, CancellationToken token)
		{
			using(await ReadLock.LockAsync(token).ConfigureAwait(false))
				return await DecoratedClient.ReadAsync(buffer, start, count, token)
					.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public override async Task ClearReadBuffers()
		{
			using(await ReadLock.LockAsync().ConfigureAwait(false))
				await DecoratedClient.ClearReadBuffers()
					.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public override async Task DisconnectAsync(int delay)
		{
			await DecoratedClient.DisconnectAsync(delay)
				.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public override async Task WriteAsync(byte[] bytes, int offset, int count)
		{
			await DecoratedClient.WriteAsync(bytes, offset, count)
				.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public override async Task<bool> ConnectAsync(string ip, int port)
		{
			return await DecoratedClient.ConnectAsync(ip, port)
				.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public async Task<NetworkIncomingMessage<TReadPayloadBaseType>> ReadAsync(CancellationToken token)
		{
			using(await ReadLock.LockAsync(token).ConfigureAwait(false))
			{
				//We want to clear the read buffers after reaiding a full message.
				NetworkIncomingMessage<TReadPayloadBaseType> message = await DecoratedClient.ReadAsync(token)
					.ConfigureAwait(false);

				//Do not call the object's ClearBuffer or we will deadlock; isn't re-enterant
				await DecoratedClient.ClearReadBuffers()
					.ConfigureAwait(false);

				return message;
			}
		}

		/// <inheritdoc />
		public async Task WriteAsync(TWritePayloadBaseType payload)
		{
			await DecoratedClient.WriteAsync(payload)
				.ConfigureAwait(false);
		}
	}
}
