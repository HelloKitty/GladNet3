using System;

namespace GladNet
{
	/// <summary>
	/// Builder for a network message client.
	/// </summary>
	/// <typeparam name="TNetworkType">The decorated network client type.</typeparam>
	public sealed class NetworkMessageClientBuilder<TNetworkType, THeaderReaderWriterType>
		where TNetworkType : NetworkClientBase
		where THeaderReaderWriterType : IPacketHeaderReadable, IPacketHeaderWritable
	{
		/// <summary>
		/// Serializer dependency for the net message client.
		/// </summary>
		public INetworkSerializationService Serializer { get; }

		/// <summary>
		/// The network client to decorate.
		/// </summary>
		public TNetworkType Client { get; }

		public THeaderReaderWriterType HeaderReaderWriter { get; }

		public NetworkMessageClientBuilder(INetworkSerializationService serializer, TNetworkType client, THeaderReaderWriterType headerReaderWriter)
		{
			if(serializer == null) throw new ArgumentNullException(nameof(serializer));
			if(client == null) throw new ArgumentNullException(nameof(client));
			if(headerReaderWriter == null) throw new ArgumentNullException(nameof(headerReaderWriter));

			Serializer = serializer;
			Client = client;
			HeaderReaderWriter = headerReaderWriter;
		}

		//Hack: Use obsolete to warn user.
		/// <summary>
		/// Allows the fallback for not calling <see cref="For{TPayloadType}"/>.
		/// </summary>
		/// <param name="builder">The builder.</param>
		[Obsolete("Did you mean to call For?")]
		public static implicit operator TNetworkType(NetworkMessageClientBuilder<TNetworkType, THeaderReaderWriterType> builder)
		{
			if(builder == null) throw new ArgumentNullException(nameof(builder));

			return builder.Client;
		}

		/// <summary>
		/// Creates a <see cref="NetworkClientBase"/> client that can handle read and writing 
		/// the specified generic <typeparamref name="TPayloadType"/>.
		/// </summary>
		/// <typeparam name="TPayloadType">The payload type.</typeparam>
		/// <returns>A network message client.</returns>
		public NetworkClientPacketPayloadReaderWriterDecorator<TNetworkType, THeaderReaderWriterType, TReadPayloadType, TWritePayloadType, TPayloadConstraintType> Build<TReadPayloadType, TWritePayloadType, TPayloadConstraintType>(IPacketHeaderFactory<TPayloadConstraintType> packetHeaderFactory) 
			where TReadPayloadType : class, TPayloadConstraintType 
			where TWritePayloadType : class, TPayloadConstraintType
		{
			return new NetworkClientPacketPayloadReaderWriterDecorator<TNetworkType, THeaderReaderWriterType, TReadPayloadType, TWritePayloadType, TPayloadConstraintType>(Client, HeaderReaderWriter, Serializer, packetHeaderFactory);
		}

		/// <summary>
		/// Creates a <see cref="NetworkClientBase"/> client that can handle read and writing 
		/// the specified generic <typeparamref name="TPayloadType"/>.
		/// </summary>
		/// <typeparam name="TPayloadType">The payload type.</typeparam>
		/// <returns>A network message client.</returns>
		public NetworkMessageClientBuilder<NetworkClientPacketPayloadReaderWriterDecorator<TNetworkType, THeaderReaderWriterType, TReadPayloadType, TWritePayloadType, TPayloadConstraintType>, TReadPayloadType, TWritePayloadType> For<TReadPayloadType, TWritePayloadType, TPayloadConstraintType>(IPacketHeaderFactory<TPayloadConstraintType> packetHeaderFactory)
			where TReadPayloadType : class, TPayloadConstraintType
			where TWritePayloadType : class, TPayloadConstraintType
		{
			return new NetworkMessageClientBuilder<NetworkClientPacketPayloadReaderWriterDecorator<TNetworkType, THeaderReaderWriterType, TReadPayloadType, TWritePayloadType, TPayloadConstraintType>, TReadPayloadType, TWritePayloadType>(new NetworkClientPacketPayloadReaderWriterDecorator<TNetworkType, THeaderReaderWriterType, TReadPayloadType, TWritePayloadType, TPayloadConstraintType>(Client, HeaderReaderWriter, Serializer, packetHeaderFactory));
		}
	}

	public sealed class NetworkMessageClientBuilder<TNetworkType, TReadPayloadType, TWritePayloadType>
		where TNetworkType : NetworkClientBase, INetworkMessageClient<TReadPayloadType, TWritePayloadType>
		where TReadPayloadType : class
		where TWritePayloadType : class
	{
		/// <summary>
		/// The network client to decorate.
		/// </summary>
		public TNetworkType Client { get; }

		public NetworkMessageClientBuilder(TNetworkType client)
		{
			if(client == null) throw new ArgumentNullException(nameof(client));

			Client = client;
		}

		//Hack: Use obsolete to warn user.
		/// <summary>
		/// Allows the fallback for not calling <see cref="For{TPayloadType}"/>.
		/// </summary>
		/// <param name="builder">The builder.</param>
		[Obsolete("Did you mean to call Build?")]
		public static implicit operator TNetworkType(NetworkMessageClientBuilder<TNetworkType, TReadPayloadType, TWritePayloadType> builder)
		{
			if(builder == null) throw new ArgumentNullException(nameof(builder));

			return builder.Client;
		}

		/// <summary>
		/// Builds and returns the network message client.
		/// </summary>
		/// <returns></returns>
		public TNetworkType Build()
		{
			return Client;
		}

		/// <summary>
		/// Decorates the network client with read buffer clearing
		/// after message reads.
		/// </summary>
		/// <returns>A new builder to continue building from.</returns>
		public NetworkMessageClientBuilder<NetworkClientNetworkMessageReadingClearBuffersAfterReadDecorator<TNetworkType, TReadPayloadType, TWritePayloadType>, TReadPayloadType, TWritePayloadType> AddReadBufferClearing()
		{
			return new NetworkMessageClientBuilder<NetworkClientNetworkMessageReadingClearBuffersAfterReadDecorator<TNetworkType, TReadPayloadType, TWritePayloadType>, TReadPayloadType, TWritePayloadType>(new NetworkClientNetworkMessageReadingClearBuffersAfterReadDecorator<TNetworkType, TReadPayloadType, TWritePayloadType>(Client));
		}
	}
}