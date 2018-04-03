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
	/// High level server client for consumption that manages the lower level networking.
	/// Provides a high level, typesafe API with simple interactions.
	/// </summary>
	public sealed class ManagedNetworkServerClient<TClientType, TPayloadWriteType, TPayloadReadType> : ManagedNetworkClientBase<TClientType, TPayloadWriteType, TPayloadReadType>, IManagedNetworkServerClient<TPayloadWriteType, TPayloadReadType>
		where TPayloadWriteType : class
		where TPayloadReadType : class
		where TClientType : class, IDisconnectable, IConnectable, IPacketPayloadWritable<TPayloadWriteType>, IPacketPayloadReadable<TPayloadReadType>
	{
		/// <inheritdoc />
		public ManagedNetworkServerClient(TClientType unmanagedClient)
			: this(unmanagedClient, new NoOpLogger())
		{

		}

		public ManagedNetworkServerClient(TClientType unmanagedClient, ILog logger)
			: base(unmanagedClient, logger)
		{
			if(unmanagedClient == null) throw new ArgumentNullException(nameof(unmanagedClient));
			if(logger == null) throw new ArgumentNullException(nameof(logger));

			//We expect this client to be connected already because it's a managed session
			isConnected = true;
		}

		/// <inheritdoc />
		public override async Task<SendResult> SendMessage<TPayloadType>(TPayloadType payload, DeliveryMethod method)
		{
			//TODO: Handle delivery method
			//TODO: What should we do when this is being called during a critical section? Won't we want to queue this up so serialization and encryption
			//doesn't block?
			await UnmanagedClient.WriteAsync(payload)
				.ConfigureAwait(false);

			return SendResult.Sent;
		}

		/// <inheritdoc />
		public override Task<NetworkIncomingMessage<TPayloadReadType>> ReadMessageAsync(CancellationToken token)
		{
			return UnmanagedClient.ReadAsync(token);
		}

		/// <inheritdoc />
		public override void Dispose(bool disposing)
		{
			if(disposing)
			{
				//TODO: Implement dispose for this
			}
		}

		/// <inheritdoc />
		public override Task<TResponseType> InterceptPayload<TResponseType>(CancellationToken cancellationToken)
		{
			throw new NotSupportedException($"TODO: Solve design conflict with intercepting. Server sessions should not be able to intercept.");
		}
	}
}
