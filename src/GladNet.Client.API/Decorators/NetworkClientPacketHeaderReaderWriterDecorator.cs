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
		private byte[] PacketHeaderBuffer { get; }

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
			PacketHeaderBuffer = new byte[headerSize];
			DecoratedClient = decoratedClient;
			Serializer = serializer;
		}

		//TODO: This is copy-pasted from above, to avoid creating tokens when we don't need them. Should we refactor?
		public override async Task<int> ReadAsync(byte[] buffer, int start, int count, CancellationToken token)
		{
			return await DecoratedClient.ReadAsync(buffer, start, count, token);
		}

		/// <inheritdoc />
		public override async Task<bool> ConnectAsync(string address, int port)
		{
			return await DecoratedClient.ConnectAsync(address, port)
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

		public async Task<IPacketHeader> ReadHeaderAsync(CancellationToken token)
		{
			//If the token is canceled just return null;
			if(token.IsCancellationRequested)
				return null;

			//If we had access to the stream we could wrap it in a reader and use it
			//without knowing the size. Since we don't have access we must manually read
			await DecoratedClient.ReadAsync(PacketHeaderBuffer, 0, PacketHeaderBuffer.Length, token)
				.ConfigureAwait(false);//TODO: How long should the timeout be if any?

			//If the token is canceled just return null;
			if(token.IsCancellationRequested)
				return null;

			//This will deserialize
			return Serializer.Deserialize<TPacketHeaderType>(PacketHeaderBuffer);
		}

		/// <inheritdoc />
		public async Task<int> WriteHeaderAsync(IPacketHeader header)
		{
			//We only need to serialize and then write
			byte[] bytes = Serializer.Serialize(header);

			await DecoratedClient.WriteAsync(bytes)
				.ConfigureAwait(false);

			return bytes.Length;
		}
	}
}
