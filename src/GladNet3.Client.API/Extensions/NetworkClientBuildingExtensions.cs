using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;

namespace GladNet
{
	public static class NetworkClientBuildingExtensions
	{
		/// <summary>
		/// Creates a managed client adapter around the provided <see cref="client"/> providing a high level API
		/// for consumption based on this simplified slimed down <see cref="IManagedNetworkClient{TPayloadWriteType,TPayloadReadType}"/>
		/// interface.
		/// </summary>
		/// <typeparam name="TReadPayloadBaseType">The read type payload (inferred)</typeparam>
		/// <typeparam name="TWritePayloadBaseType">The write type payload (inferred)</typeparam>
		/// <param name="client">The client to adapt.</param>
		/// <returns>A new managed client.</returns>
		public static IManagedNetworkClient<TWritePayloadBaseType, TReadPayloadBaseType> AsManaged<TReadPayloadBaseType, TWritePayloadBaseType>(this INetworkMessageClient<TReadPayloadBaseType, TWritePayloadBaseType> client)
			where TWritePayloadBaseType : class
			where TReadPayloadBaseType : class
		{
			if(client == null) throw new ArgumentNullException(nameof(client));

			//Adapt the provided network client to the managed network client interfaces.
			return new ManagedNetworkClient<INetworkMessageClient<TReadPayloadBaseType, TWritePayloadBaseType>, TWritePayloadBaseType, TReadPayloadBaseType>(client);
		}

		/// <summary>
		/// Creates a managed client adapter around the provided <see cref="client"/> providing a high level API
		/// for consumption based on this simplified slimed down <see cref="IManagedNetworkClient{TPayloadWriteType,TPayloadReadType}"/>
		/// interface.
		/// </summary>
		/// <typeparam name="TReadPayloadBaseType">The read type payload (inferred)</typeparam>
		/// <typeparam name="TWritePayloadBaseType">The write type payload (inferred)</typeparam>
		/// <param name="client">The client to adapt.</param>
		/// <param name="logger"></param>
		/// <returns>A new managed client.</returns>
		public static IManagedNetworkClient<TWritePayloadBaseType, TReadPayloadBaseType> AsManaged<TReadPayloadBaseType, TWritePayloadBaseType>(this INetworkMessageClient<TReadPayloadBaseType, TWritePayloadBaseType> client, ILog logger)
			where TWritePayloadBaseType : class
			where TReadPayloadBaseType : class
		{
			if(client == null) throw new ArgumentNullException(nameof(client));
			if(logger == null) throw new ArgumentNullException(nameof(logger));

			//Adapt the provided network client to the managed network client interfaces.
			return new ManagedNetworkClient<INetworkMessageClient<TReadPayloadBaseType, TWritePayloadBaseType>, TWritePayloadBaseType, TReadPayloadBaseType>(client, logger);
		}

		/// <summary>
		/// Enables crypt handling for the client.
		/// </summary>
		/// <typeparam name="TNetworkClientType">The client type.</typeparam>
		/// <param name="client">The client to add crypt handling to.</param>
		/// <param name="encryptionService">The service used to encrypt.</param>
		/// <param name="decryptionService">The service used to decrypt.</param>
		/// <returns>A client with crypto handling functionality.</returns>
		public static NetworkClientBase AddCryptHandling<TNetworkClientType>(this TNetworkClientType client, ICryptoServiceProvider encryptionService, ICryptoServiceProvider decryptionService)
			where TNetworkClientType : NetworkClientBase
		{
			if(client == null) throw new ArgumentNullException(nameof(client));
			if(encryptionService == null) throw new ArgumentNullException(nameof(encryptionService));
			if(decryptionService == null) throw new ArgumentNullException(nameof(decryptionService));

			//TODO: Check the blocksize for both. We don't support different blocksizes yet
			if(encryptionService.BlockSize != 0 && encryptionService.BlockSize != 1)
				return new NetworkClientFixedBlockSizeCryptoDecorator(client, encryptionService, decryptionService, encryptionService.BlockSize);

			return new NetworkClientBlocklessCryptoDecorator(client, encryptionService, decryptionService);
		}

		/// <summary>
		/// Enables header reading functionality to a client.
		/// </summary>
		/// <param name="client">The client to add header reading to.</param>
		/// <returns>A client that can read headers from the network.</returns>
		public static NetworkClientPacketHeaderReaderWriterDecorator<TPacketHeaderType> AddHeaderReading<TPacketHeaderType>(this NetworkClientBase client, INetworkSerializationService serializer, int headerSize) 
			where TPacketHeaderType : IPacketHeader
		{
			if(client == null) throw new ArgumentNullException(nameof(client));
			if(serializer == null) throw new ArgumentNullException(nameof(serializer));

			return new NetworkClientPacketHeaderReaderWriterDecorator<TPacketHeaderType>(client, serializer, headerSize);
		}

		/// <summary>
		/// Enables header reading functionality to a client.
		/// </summary>
		/// <typeparam name="TNetworkClientType">The client type.</typeparam>
		/// <param name="client">The client to add header reading to.</param>
		/// <returns>A client that can read headers from the network.</returns>
		public static NetworkMessageClientBuilder<TNetworkClientType, TNetworkClientType> AddNetworkMessageReading<TNetworkClientType>(this TNetworkClientType client, INetworkSerializationService serializer)
			where TNetworkClientType : NetworkClientBase, IPacketHeaderReadable, IPacketHeaderWritable
		{
			if(client == null) throw new ArgumentNullException(nameof(client));
			if(serializer == null) throw new ArgumentNullException(nameof(serializer));

			//New netmessage client builder
			return new NetworkMessageClientBuilder<TNetworkClientType, TNetworkClientType>(serializer, client, client);
		}

		/// <summary>
		/// Enables bufferred writing functionality. This means that the actual writing won't begin until the
		/// provided buffer threshold <see cref="bufferedCount"/> is reached.
		/// </summary>
		/// <param name="client">The client to decorate.</param>
		/// <param name="bufferedCount">The amount to wait for before writing.</param>
		/// <returns>The client decorated with buffered write functionality</returns>
		public static NetworkClientBufferWriteUntilSizeReachedDecorator AddBufferredWrite(this NetworkClientBase client, int bufferedCount)
		{
			if(client == null) throw new ArgumentNullException(nameof(client));
			if(bufferedCount <= 0) throw new ArgumentOutOfRangeException(nameof(bufferedCount));

			return new NetworkClientBufferWriteUntilSizeReachedDecorator(client, bufferedCount);
		}

		/// <summary>
		/// Enables clearing the read buffer after a full message has been read.
		/// </summary>
		/// <typeparam name="TClientType"></typeparam>
		/// <typeparam name="TReadPayloadBaseType"></typeparam>
		/// <typeparam name="TWritePayloadBaseType"></typeparam>
		/// <param name="client"></param>
		/// <returns></returns>
		public static NetworkClientNetworkMessageReadingClearBuffersAfterReadDecorator<TClientType, TReadPayloadBaseType, TWritePayloadBaseType> AddReadBufferClearingOnMessageRead<TClientType, TReadPayloadBaseType, TWritePayloadBaseType>(this TClientType client)
			where TClientType : NetworkClientBase, INetworkMessageClient<TReadPayloadBaseType, TWritePayloadBaseType> 
			where TWritePayloadBaseType : class
			where TReadPayloadBaseType : class
		{
			if(client == null) throw new ArgumentNullException(nameof(client));

			return new NetworkClientNetworkMessageReadingClearBuffersAfterReadDecorator<TClientType, TReadPayloadBaseType, TWritePayloadBaseType>(client);
		}
	}
}
