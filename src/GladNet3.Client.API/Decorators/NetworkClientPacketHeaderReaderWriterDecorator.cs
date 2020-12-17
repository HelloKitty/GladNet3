using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace GladNet
{
	/// <summary>
	/// Decorator that adds header reading functionality to the <see cref="NetworkClientBase"/>.
	/// </summary>
	public sealed class NetworkClientPacketHeaderReaderWriterDecorator<TPacketHeaderType> : NetworkClientBase, IPacketHeaderReadable, IPacketHeaderWritable
		where TPacketHeaderType : IPacketHeader
	{
		/// <summary>
		/// The decorated <see cref="NetworkClientBase"/>.
		/// </summary>
		private NetworkClientBase DecoratedClient { get; }

		/// <summary>
		/// The serialization service.
		/// </summary>
		private INetworkSerializationService Serializer { get; }

		//TODO: Thread safety
		/// <summary>
		/// Thread specific buffer used to deserialize the packet header bytes into.
		/// </summary>
		private byte[] PacketHeaderReadBuffer { get; }

		private byte[] PacketHeaderWriteBuffer { get; }

		/// <summary>
		/// </summary>
		/// <param name="decoratedClient">The client to decorate.</param>
		/// <param name="serializer"></param>
		/// <param name="headerSize"></param>
		public NetworkClientPacketHeaderReaderWriterDecorator(NetworkClientBase decoratedClient, INetworkSerializationService serializer, int headerSize)
		{
			if(decoratedClient == null) throw new ArgumentNullException(nameof(decoratedClient));
			if(serializer == null) throw new ArgumentNullException(nameof(serializer));
			if(headerSize < 0) throw new ArgumentOutOfRangeException(nameof(headerSize));

			//We need to support up to the maximum block
			PacketHeaderReadBuffer = new byte[headerSize];
			PacketHeaderWriteBuffer = new byte[headerSize];
			DecoratedClient = decoratedClient;
			Serializer = serializer;
		}

		//TODO: This is copy-pasted from above, to avoid creating tokens when we don't need them. Should we refactor?
		public override Task<int> ReadAsync(byte[] buffer, int start, int count, CancellationToken token)
		{
			return DecoratedClient.ReadAsync(buffer, start, count, token);
		}

		/// <inheritdoc />
		public override Task ClearReadBuffers()
		{
			return DecoratedClient.ClearReadBuffers();
		}

		/// <inheritdoc />
		public override Task<bool> ConnectAsync(string address, int port)
		{
			return DecoratedClient.ConnectAsync(address, port)
;
		}

		/// <inheritdoc />
		public override Task DisconnectAsync(int delay)
		{
			return DecoratedClient.DisconnectAsync(delay);
		}

		/// <inheritdoc />
		public override Task WriteAsync(byte[] bytes, int offset, int count)
		{
			return DecoratedClient.WriteAsync(bytes, offset, count);
		}

		public async Task<IPacketHeader> ReadHeaderAsync(CancellationToken token)
		{
			//If the token is canceled just return null;
			if(token.IsCancellationRequested)
				return null;

			//If we had access to the stream we could wrap it in a reader and use it
			//without knowing the size. Since we don't have access we must manually read
			int count = await DecoratedClient.ReadAsync(PacketHeaderReadBuffer, 0, PacketHeaderReadBuffer.Length, token)
				.ConfigureAwait(false);//TODO: How long should the timeout be if any?

			//This means the socket is disconnected
			if(count == 0)
				return null;

			//If the token is canceled just return null;
			if(token.IsCancellationRequested)
				return null;

			//This will deserialize
			return Serializer.Deserialize<TPacketHeaderType>(PacketHeaderReadBuffer);
		}

		/// <inheritdoc />
		public async Task<int> WriteHeaderAsync(IPacketHeader header)
		{
			//We only need to serialize and then write
			int count = Serializer.Serialize(header, PacketHeaderWriteBuffer);

			await DecoratedClient.WriteAsync(PacketHeaderWriteBuffer, 0, count)
				.ConfigureAwait(false);

			return count;
		}
	}
}
