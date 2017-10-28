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
		public NetworkClientPacketPayloadReaderWriterDecorator<TNetworkType, THeaderReaderWriterType, TReadPayloadType, TWritePayloadType, TPayloadConstraintType> For<TReadPayloadType, TWritePayloadType, TPayloadConstraintType>(IPacketHeaderFactory<TPayloadConstraintType> packetHeaderFactory) 
			where TReadPayloadType : class, TPayloadConstraintType 
			where TWritePayloadType : class, TPayloadConstraintType
		{
			return new NetworkClientPacketPayloadReaderWriterDecorator<TNetworkType, THeaderReaderWriterType, TReadPayloadType, TWritePayloadType, TPayloadConstraintType>(Client, HeaderReaderWriter, Serializer, packetHeaderFactory);
		}
	}
}