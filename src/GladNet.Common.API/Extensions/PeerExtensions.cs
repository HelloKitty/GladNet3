using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GladNet
{
	public static class PeerExtensions
	{
		//This extension exists mostly to support the old TCP only API
		/// <summary>
		/// Sends the provided <see cref="payload"/>
		/// </summary>
		/// <typeparam name="TPayloadType">The type of payload.</typeparam>
		/// <typeparam name="TPayloadBaseType">The base type of the payload.</typeparam>
		/// <param name="sendService">The extended send service.</param>
		/// <param name="payload">The payload to send.</param>
		/// <returns>Indicates the result of the send message operation.</returns>
		public static async Task<SendResult> SendMessage<TPayloadBaseType, TPayloadType>(this IPeerPayloadSendService<TPayloadBaseType> sendService, TPayloadType payload)
			where TPayloadType : class, TPayloadBaseType 
			where TPayloadBaseType : class
		{
			if(sendService == null) throw new ArgumentNullException(nameof(sendService));
			if(payload == null) throw new ArgumentNullException(nameof(payload));

			//We default to reliable ordered as if this is TCP
			return await sendService.SendMessage(payload, DeliveryMethod.ReliableOrdered);
		}

		//This extension mostly exists for the old TCP-only API
		/// <summary>
		/// Sends the <see cref="request"/> payload and provided a future awaitable
		/// that can yield the <typeparamref name="TResponseType"/> payload.
		/// </summary>
		/// <typeparam name="TResponseType"></typeparam>
		/// <typeparam name="TPayloadBaseType"></typeparam>
		/// <param name="sendService"></param>
		/// <param name="request">The request payload.</param>
		/// <returns>A future that contains the response.</returns>
		public static async Task<TResponseType> SendRequestAsync<TPayloadBaseType, TResponseType>(this IPeerRequestSendService<TPayloadBaseType> sendService, TPayloadBaseType request) 
			where TPayloadBaseType : class
		{
			if(sendService == null) throw new ArgumentNullException(nameof(sendService));
			if(request == null) throw new ArgumentNullException(nameof(request));

			//Since no delivry or cancel was provided we should default to reliable and also make sure there is no cancel
			return await sendService.SendRequestAsync<TResponseType>(request, DeliveryMethod.ReliableOrdered, CancellationToken.None);
		}

		/// <summary>
		/// Attempts to read a <see cref="IPacketHeader"/> from
		/// the client.
		/// </summary>
		/// <returns>A PSOBBPacketHeader.</returns>
		public static async Task<IPacketHeader> ReadHeaderAsync(this IPacketHeaderReadable packetHeaderReadable)
		{
			if(packetHeaderReadable == null) throw new ArgumentNullException(nameof(packetHeaderReadable));

			return await packetHeaderReadable.ReadHeaderAsync(CancellationToken.None);
		}

		/// <summary>
		/// Attempts to read a <see cref="IPacketHeader"/> from
		/// the client.
		/// </summary>
		/// <returns>A PSOBBPacketHeader.</returns>
		public static IPacketHeader ReadHeader(this IPacketHeaderReadable packetHeaderReadable)
		{
			return packetHeaderReadable.ReadHeaderAsync().Result;
		}

		/// <summary>
		/// Reads an incoming message syncronously and blocks until it recieves one.
		/// </summary>
		/// <returns>The resulting incoming message.</returns>
		public static NetworkIncomingMessage<TPayloadBaseType> Read<TPayloadBaseType>(this IPacketPayloadReadable<TPayloadBaseType> readable) 
			where TPayloadBaseType : class
		{
			if(readable == null) throw new ArgumentNullException(nameof(readable));

			return readable.ReadAsync().Result;
		}

		/// <summary>
		/// Reads an incoming message asyncronously.
		/// The task will complete when an incomding message can be built.
		/// </summary>
		/// <returns>A future for the resulting incoming message.</returns>
		public static async Task<NetworkIncomingMessage<TPayloadBaseType>> ReadAsync<TPayloadBaseType>(this IPacketPayloadReadable<TPayloadBaseType> readable) 
			where TPayloadBaseType : class
		{
			if(readable == null) throw new ArgumentNullException(nameof(readable));

			return await readable.ReadAsync(CancellationToken.None);
		}

		/// <summary>
		/// Writes the provided <see cref="payload"/>.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="payload">The payload to write.</param>
		public static void Write<TPayloadBaseType>(this IPacketPayloadWritable<TPayloadBaseType> writer, TPayloadBaseType payload)
			where TPayloadBaseType : class
		{
			if(writer == null) throw new ArgumentNullException(nameof(writer));

			//Don't await the task.
			writer.WriteAsync(payload);
		}

		//TODO: Add cancellation token support
		/// <summary>
		/// Registers an interception request that yields an awaitable for
		/// the specified <typeparamref name="TResponseType"/> type.
		/// </summary>
		/// <typeparam name="TResponseType">The payload type to intercept.</typeparam>
		/// <returns>An awaitable for the next recieved payload of the speified type.</returns>
		public static async Task<TResponseType> InterceptPayload<TResponseType>(this IPayloadInterceptable interceptable)
		{
			return await interceptable.InterceptPayload<TResponseType>(CancellationToken.None);
		}

		/// <summary>
		/// Connects to the provided <see cref="address"/> with on the given <see cref="port"/>.
		/// </summary>
		/// <param name="connectable"></param>
		/// <param name="address">The ip.</param>
		/// <param name="port">The port.</param>
		/// <returns>True if connection was successful.</returns>
		public static bool Connect(this IConnectable connectable, IPAddress address, int port)
		{
			if(connectable == null) throw new ArgumentNullException(nameof(connectable));
			if(address == null) throw new ArgumentNullException(nameof(address));
			if(port < 0) throw new ArgumentOutOfRangeException(nameof(port));

			return connectable.Connect(address.ToString(), port);
		}

		/// <summary>
		/// Connects to the provided <see cref="address"/> with on the given <see cref="port"/>.
		/// </summary>
		/// <param name="connectable"></param>
		/// <param name="address">The ip.</param>
		/// <param name="port">The port.</param>
		/// <returns>True if connection was successful.</returns>
		public static async Task<bool> ConnectAsync(this IConnectable connectable, IPAddress address, int port)
		{
			if(connectable == null) throw new ArgumentNullException(nameof(connectable));
			if(address == null) throw new ArgumentNullException(nameof(address));
			if(port < 0) throw new ArgumentOutOfRangeException(nameof(port));

			return await connectable.ConnectAsync(address.ToString(), port);
		}

		/// <summary>
		/// Connects to the provided <see cref="ip"/> with on the given <see cref="port"/>.
		/// </summary>
		/// <param name="connectable"></param>
		/// <param name="ip">The ip.</param>
		/// <param name="port">The port.</param>
		/// <returns>True if connection was successful.</returns>
		public static bool Connect(this IConnectable connectable, string ip, int port)
		{
			if(connectable == null) throw new ArgumentNullException(nameof(connectable));
			if(string.IsNullOrWhiteSpace(ip)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(ip));
			if(port < 0) throw new ArgumentOutOfRangeException(nameof(port));

			return connectable.ConnectAsync(ip, port).Result;
		}

		/// <summary>
		/// Disconnects the object from it's connected source.
		/// </summary>
		public static void Disconnect(this IDisconnectable disconnectable)
		{
			if(disconnectable == null) throw new ArgumentNullException(nameof(disconnectable));

			disconnectable.DisconnectAsync(0).Wait();
		}
	}
}
