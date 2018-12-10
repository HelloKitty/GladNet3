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
			if(this.isConnected)
			{
				try
				{
					//TODO: Handle delivery method
					//TODO: What should we do when this is being called during a critical section? Won't we want to queue this up so serialization and encryption
					//doesn't block?
					await UnmanagedClient.WriteAsync(payload)
						.ConfigureAwait(false);
				}
				catch(NetworkDisconnectedException e)
				{
					//The client/session/network that the caller
					//was trying to send a message to is disconnected. This doesn't mean
					//we want to throw though. Catching exceptions is EXPENSIVE but this should only happen on a rare occasion
					//and the caller will see that the client is disconnected, and make decisions based on that
					//and of course disconnection logic will likely be happening else where during this
					return SendResult.Disconnected;
				}

				//We should let other exceptions be thrown though, as they aren't related to connectivity.
			}
			else
				return SendResult.Disconnected;

			return SendResult.Sent;
		}

		//ServerClient SendMessage already sends right away. So we can just call it
		/// <inheritdoc />
		public override Task<SendResult> SendMessageImmediately<TPayloadType>(TPayloadType payload, DeliveryMethod method)
		{
			//TODO: Handle delivery method
			//TODO: What should we do when this is being called during a critical section? Won't we want to queue this up so serialization and encryption
			return SendMessage(payload, method);
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
