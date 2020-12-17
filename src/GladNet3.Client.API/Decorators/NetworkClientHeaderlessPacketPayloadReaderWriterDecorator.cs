using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace GladNet
{
	/// <summary>
	/// Decorator that decorates the provided <see cref="NetworkClientBase"/> with functionality
	/// that allows you to write <see cref="TWritePayloadBaseType"/> directly into the stream/client.
	/// Overloads the usage of <see cref="Write"/> to accomplish this.
	/// </summary>
	/// <typeparam name="TClientType">The type of decorated client.</typeparam>
	/// <typeparam name="TWritePayloadBaseType"></typeparam>
	/// <typeparam name="TReadPayloadBaseType"></typeparam>
	/// <typeparam name="TPayloadConstraintType">The constraint requirement for </typeparam>
	public sealed class NetworkClientHeaderlessPacketPayloadReaderWriterDecorator<TClientType, TReadPayloadBaseType, TWritePayloadBaseType, TPayloadConstraintType> : NetworkClientBase,
		INetworkMessageClient<TReadPayloadBaseType, TWritePayloadBaseType>, IDisposable
		where TClientType : NetworkClientBase
		where TReadPayloadBaseType : class, TPayloadConstraintType
		where TWritePayloadBaseType : class, TPayloadConstraintType
	{
		/// <summary>
		/// The decorated client.
		/// </summary>
		private TClientType DecoratedClient { get; }

		/// <summary>
		/// The serializer service.
		/// </summary>
		private INetworkSerializationService Serializer { get; }

		/// <summary>
		/// Thread specific buffer used to deserialize the packet bytes into.
		/// </summary>
		private byte[] PacketPayloadReadBuffer { get; }

		/// <summary>
		/// Thread specific buffer used to serializer the packet bytes.
		/// </summary>
		private byte[] PacketPayloadWriteBuffer { get; }

		/// <summary>
		/// Async read syncronization object.
		/// </summary>
		private readonly AsyncLock readSynObj = new AsyncLock();

		/// <summary>
		/// Async write syncronization object.
		/// </summary>
		private readonly AsyncLock writeSynObj = new AsyncLock();

		public NetworkClientHeaderlessPacketPayloadReaderWriterDecorator(TClientType decoratedClient, INetworkSerializationService serializer, int payloadBufferSize = 30000)
		{
			if(decoratedClient == null) throw new ArgumentNullException(nameof(decoratedClient));
			if(serializer == null) throw new ArgumentNullException(nameof(serializer));
			if(payloadBufferSize <= 0) throw new ArgumentOutOfRangeException(nameof(payloadBufferSize));

			DecoratedClient = decoratedClient;
			Serializer = serializer;

			//One of the lobby packets is 14,000 bytes. We may even need bigger.
			PacketPayloadReadBuffer = new byte[payloadBufferSize]; //TODO: Do we need a larger buffer for any packets?
			PacketPayloadWriteBuffer = new byte[payloadBufferSize];
		}

		/// <inheritdoc />
		public override Task<bool> ConnectAsync(string address, int port)
		{
			return DecoratedClient.ConnectAsync(address, port);
		}

		/// <inheritdoc />
		public override async Task ClearReadBuffers()
		{
			using(await readSynObj.LockAsync().ConfigureAwait(false))
				await DecoratedClient.ClearReadBuffers()
					.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public override Task DisconnectAsync(int delay)
		{
			return DecoratedClient.DisconnectAsync(delay);
		}

		/// <inheritdoc />
		public void Write(TWritePayloadBaseType payload)
		{
			//Write the outgoing message, it will internally create the header and it will be serialized
			WriteAsync(payload).Wait();
		}

		/// <inheritdoc />
		public override Task WriteAsync(byte[] bytes, int offset, int count)
		{
			return DecoratedClient.WriteAsync(bytes, offset, count);
		}

		/// <inheritdoc />
		public async Task WriteAsync(TWritePayloadBaseType payload)
		{
			using(await writeSynObj.LockAsync().ConfigureAwait(false))
			{
				//Serializer the payload first so we can build the header
				int size = Serializer.Serialize(payload, new Span<byte>(PacketPayloadWriteBuffer));

				//Write the outgoing message
				await DecoratedClient.WriteAsync(PacketPayloadWriteBuffer, 0, size)
					.ConfigureAwait(false);
			}
		}

		/// <inheritdoc />
		public override Task<int> ReadAsync(byte[] buffer, int start, int count, CancellationToken token)
		{
			return DecoratedClient.ReadAsync(buffer, start, count, token);
		}

		public async Task<NetworkIncomingMessage<TReadPayloadBaseType>> ReadAsync(CancellationToken token)
		{
			TReadPayloadBaseType payload = null;

			IPacketHeader header = null;

			using(await readSynObj.LockAsync(token).ConfigureAwait(false))
			{
				//if was canceled the header reading probably returned null anyway
				if(token.IsCancellationRequested)
					return null;

				//We need to read enough bytes to deserialize the payload
				throw new NotSupportedException($"TODO: Support non-length prefixed network messages again.");
				//payload = await Serializer.Deserialize<TReadPayloadBaseType>(this, token)
				//	.ConfigureAwait(false); //TODO: Should we timeout?

				//Null payload means the socket disconnected
				if(payload == null)
					return null;

				//TODO: Any reasonable way to get the bytes read size so we can use it as the header?
				//We need to create our own manual header since we aren't reading one
				//However, we do not know the header size so we should just say it's 0
				header = new HeaderlessPacketHeader(0);
			}

			//If the token was canceled then the buffer isn't filled and we can't make a message
			if(token.IsCancellationRequested)
				return null;

			return new NetworkIncomingMessage<TReadPayloadBaseType>(header, payload);
		}
	}
}
