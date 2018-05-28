using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using Common.Logging;
using Common.Logging.Simple;

namespace GladNet
{
	/// <summary>
	/// High level client for consumption that manages the lower level networking.
	/// Provides a high level, typesafe API with simple interactions.
	/// </summary>
	public abstract class ManagedNetworkClientBase<TClientType, TPayloadWriteType, TPayloadReadType> : IManagedNetworkClient<TPayloadWriteType, TPayloadReadType>, IDisposable
		where TPayloadWriteType : class
		where TPayloadReadType : class
		where TClientType : class, IDisconnectable, IConnectable, IPacketPayloadWritable<TPayloadWriteType>, IPacketPayloadReadable<TPayloadReadType>
	{
		//TODO: Syncronization maybe? Lots of issues could come up if connect or disconnect
		//is called at the same time.
		/// <inheritdoc />
		public bool isConnected { get; protected set; }

		//Not unmanaged in the C vs C# sense but unmanaged meaning that it doesn't really do anything
		//Unless interacted with or managed.
		/// <summary>
		/// The network client.
		/// </summary>
		protected TClientType UnmanagedClient { get; }

		//TODO: Add indepth client logging.
		/// <summary>
		/// The client logger.
		/// </summary>
		protected ILog Logger { get; }

		/// <inheritdoc />
		protected ManagedNetworkClientBase(TClientType unmanagedClient)
			: this(unmanagedClient, new NoOpLogger())
		{

		}

		protected ManagedNetworkClientBase(TClientType unmanagedClient, ILog logger)
		{
			if(unmanagedClient == null) throw new ArgumentNullException(nameof(unmanagedClient));
			if(logger == null) throw new ArgumentNullException(nameof(logger));

			UnmanagedClient = unmanagedClient;
			Logger = logger;
		}

		/// <inheritdoc />
		public abstract Task<SendResult> SendMessage<TPayloadType>(TPayloadType payload, DeliveryMethod method)
			where TPayloadType : class, TPayloadWriteType;

		/// <summary>
		/// Similar to <see cref="SendMessage{TPayloadType}"/> but will noy rely on any queuing mechanism
		/// should one be implemented under <see cref="SendMessage{TPayloadType}"/>. It instead will
		/// send the payload and message immediately and the awaited task will complete once the message is fully sent.
		/// <see cref="SendMessage{TPayloadType}"/> does not make that promise. It may complete after queueing, potentially.
		/// </summary>
		/// <typeparam name="TPayloadType">The payload type to send. (Can be inferred)</typeparam>
		/// <param name="payload">The payload.</param>
		/// <param name="method">The delivery method.</param>
		/// <returns></returns>
		public abstract Task<SendResult> SendMessageImmediately<TPayloadType>(TPayloadType payload, DeliveryMethod method)
			where TPayloadType : class, TPayloadWriteType;

		/// <inheritdoc />
		public abstract Task<NetworkIncomingMessage<TPayloadReadType>> ReadMessageAsync(CancellationToken token);

		/// <inheritdoc />
		public virtual async Task<bool> ConnectAsync(string address, int port)
		{
			//Disconnect if we're already connected
			if(isConnected)
				await DisconnectAsync(0)
					.ConfigureAwait(false);

			//This COULD return false, so we need to handle that
			isConnected = await UnmanagedClient.ConnectAsync(address, port)
				.ConfigureAwait(false);

			return isConnected;
		}

		/// <inheritdoc />
		public virtual async Task DisconnectAsync(int delay)
		{
			await UnmanagedClient.DisconnectAsync(delay)
				.ConfigureAwait(false);

			isConnected = false;
		}

		protected bool disposedValue = false; // To detect redundant calls

		public abstract void Dispose(bool disposing);

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}

		/// <inheritdoc />
		public abstract Task<TResponseType> InterceptPayload<TResponseType>(CancellationToken cancellationToken);
	}
}
