﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GladNet
{
	/// <summary>
	/// Generic payload-based implementation of <see cref="ManagedSession{TPayloadReadType,TPayloadWriteType}"/>
	/// that implements basic functionality for reading/writing payload-based messages.
	/// </summary>
	/// <typeparam name="TPayloadReadType">The type of payload being read.</typeparam>
	/// <typeparam name="TPayloadWriteType">The type of payload being written.</typeparam>
	public abstract class ManagedSession<TPayloadReadType, TPayloadWriteType> 
		: ManagedSession, INetworkMessageReceivable<TPayloadReadType>
		where TPayloadReadType : class 
		where TPayloadWriteType : class
	{
		/// <summary>
		/// The service that provides an interface for communication for reading and writing payloads.
		/// </summary>
		protected INetworkMessageInterface<TPayloadReadType, TPayloadWriteType> NetworkMessageInterface { get; }

		//These aren't currently used directly in the session themselves but we expose them incase implementer
		//wants them.
		/// <summary>
		/// Message serialization/building services.
		/// </summary>
		protected SessionMessageBuildingServiceContext<TPayloadWriteType, TPayloadReadType> MessageBuilders { get; }

		protected ManagedSession(INetworkConnectionService connectionService, SessionDetails details, NetworkConnectionOptions networkOptions,
			INetworkMessageInterface<TPayloadReadType, TPayloadWriteType> networkMessageProducer, 
			SessionMessageBuildingServiceContext<TPayloadWriteType, TPayloadReadType> messageBuilders) 
			: base(connectionService, details, networkOptions)
		{
			NetworkMessageInterface = networkMessageProducer ?? throw new ArgumentNullException(nameof(networkMessageProducer));
			MessageBuilders = messageBuilders ?? throw new ArgumentNullException(nameof(messageBuilders));
		}

		public override async Task StartListeningAsync(CancellationToken token = default)
		{
			while(!token.IsCancellationRequested)
			{
				//The producer service knows HOW to generate a message, all we need to do is await it and dispatch it to the session.
				NetworkIncomingMessage<TPayloadReadType> message = await NetworkMessageInterface.ReadMessageAsync(token);

				//TODO: Returning null to indicate failure is kinda DUMB.
				if(message == null)
					return;

				//A throw will stop the session.
				await OnNetworkMessageReceivedAsync(message, token);
			}
		}

		//Warning to implementer, if you THROW from this you WILL stop the network connection completely.
		//GladNet does not sustain exceptions in unexpected cases, choosing to shutdown the session instead.
		/// <inheritdoc />
		public abstract Task OnNetworkMessageReceivedAsync(NetworkIncomingMessage<TPayloadReadType> message, CancellationToken token = default(CancellationToken));
	}
}
