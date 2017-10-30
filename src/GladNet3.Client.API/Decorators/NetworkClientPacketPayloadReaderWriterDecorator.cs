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
	/// <typeparam name="THeaderReaderWriterType"></typeparam>
	public sealed class NetworkClientPacketPayloadReaderWriterDecorator<TClientType, THeaderReaderWriterType, TReadPayloadBaseType, TWritePayloadBaseType, TPayloadConstraintType> : NetworkClientBase,
		INetworkMessageClient<TReadPayloadBaseType, TWritePayloadBaseType>
		where THeaderReaderWriterType : IPacketHeaderReadable, IPacketHeaderWritable
		where TClientType : NetworkClientBase
		where TReadPayloadBaseType : class, TPayloadConstraintType
		where TWritePayloadBaseType : class, TPayloadConstraintType
	{
		/// <summary>
		/// The decorated client.
		/// </summary>
		private TClientType DecoratedClient { get; }

		/// <summary>
		/// Service that readers and writers packet headers.
		/// </summary>
		private THeaderReaderWriterType HeaderReaderWriter { get; }

		/// <summary>
		/// The serializer service.
		/// </summary>
		private INetworkSerializationService Serializer { get; }

		/// <summary>
		/// Thread specific buffer used to deserialize the packet header bytes into.
		/// </summary>
		private byte[] PacketPayloadReadBuffer { get; }

		private IPacketHeaderFactory<TPayloadConstraintType> PacketHeaderFactory { get; }

		/// <summary>
		/// Async read syncronization object.
		/// </summary>
		private readonly AsyncLock readSynObj = new AsyncLock();

		/// <summary>
		/// Async write syncronization object.
		/// </summary>
		private readonly AsyncLock writeSynObj = new AsyncLock();

		public NetworkClientPacketPayloadReaderWriterDecorator(TClientType decoratedClient, THeaderReaderWriterType headerReaderWriter, INetworkSerializationService serializer, IPacketHeaderFactory<TPayloadConstraintType> packetHeaderFactory, int payloadBufferSize = 30000)
		{
			if(decoratedClient == null) throw new ArgumentNullException(nameof(decoratedClient));
			if(serializer == null) throw new ArgumentNullException(nameof(serializer));
			if(packetHeaderFactory == null) throw new ArgumentNullException(nameof(packetHeaderFactory));
			if(payloadBufferSize <= 0) throw new ArgumentOutOfRangeException(nameof(payloadBufferSize));

			DecoratedClient = decoratedClient;
			Serializer = serializer;
			PacketHeaderFactory = packetHeaderFactory;
			HeaderReaderWriter = headerReaderWriter;

			//One of the lobby packets is 14,000 bytes. We may even need bigger.
			PacketPayloadReadBuffer = new byte[payloadBufferSize]; //TODO: Do we need a larger buffer for any packets?
		}

		/// <inheritdoc />
		public override async Task<bool> ConnectAsync(string address, int port)
		{
			return await DecoratedClient.ConnectAsync(address, port)
				.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public override async Task ClearReadBuffers()
		{
			using(await readSynObj.LockAsync().ConfigureAwait(false))
				await DecoratedClient.ClearReadBuffers();
		}

		/// <inheritdoc />
		public override async Task DisconnectAsync(int delay)
		{
			await DecoratedClient.DisconnectAsync(delay)
				.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public void Write(TWritePayloadBaseType payload)
		{
			//Write the outgoing message, it will internally create the header and it will be serialized
			WriteAsync(payload).Wait();
		}

		/// <inheritdoc />
		public override async Task WriteAsync(byte[] bytes, int offset, int count)
		{
			await DecoratedClient.WriteAsync(bytes, offset, count)
				.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public async Task WriteAsync(TWritePayloadBaseType payload)
		{
			//Serializer the payload first so we can build the header
			byte[] payloadData = Serializer.Serialize(payload);

			IPacketHeader header = PacketHeaderFactory.Create(payload, payloadData);

			//VERY critical we lock here otherwise we could write a header and then another unrelated body could be written inbetween
			using(await writeSynObj.LockAsync().ConfigureAwait(false))
			{
				//It's important to always write the header first
				await HeaderReaderWriter.WriteHeaderAsync(header)
					.ConfigureAwait(false);

				//Write the outgoing message, it will internally create the header and it will be serialized
				await DecoratedClient.WriteAsync(payloadData)
					.ConfigureAwait(false);
			}
		}

		/// <inheritdoc />
		public override async Task<int> ReadAsync(byte[] buffer, int start, int count, CancellationToken token)
		{
			return await DecoratedClient.ReadAsync(buffer, start, count, token)
				.ConfigureAwait(false);
		}

		public async Task<NetworkIncomingMessage<TReadPayloadBaseType>> ReadAsync(CancellationToken token)
		{
			IPacketHeader header = null;

			using(await readSynObj.LockAsync(token).ConfigureAwait(false))
			{
				//Read the header first
				header = await HeaderReaderWriter.ReadHeaderAsync(token)
					.ConfigureAwait(false);

				//if was canceled the header reading probably returned null anyway
				if(token.IsCancellationRequested)
					return null;

				//We need to read enough bytes to deserialize the payload
				await ReadAsync(PacketPayloadReadBuffer, 0, header.PayloadSize, token)
					.ConfigureAwait(false);//TODO: Should we timeout?
			}

			//If the token was canceled then the buffer isn't filled and we can't make a message
			if(token.IsCancellationRequested)
				return null;

			//Deserialize the bytes starting from the begining but ONLY read up to the payload size. We reuse this buffer and it's large
			//so if we don't specify the length we could end up with an issue.
			TReadPayloadBaseType payload = Serializer.Deserialize<TReadPayloadBaseType>(PacketPayloadReadBuffer, 0, header.PayloadSize);

			return new NetworkIncomingMessage<TReadPayloadBaseType>(header, payload);
		}
	}
}
