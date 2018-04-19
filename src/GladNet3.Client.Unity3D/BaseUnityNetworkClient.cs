using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Common.Logging;
using GladNet;
using UnityEngine;

namespace GladNet3
{
	/// <summary>
	/// Abstract base network client for Unity3D.
	/// </summary>
	/// <typeparam name="TIncomingPayloadType"></typeparam>
	/// <typeparam name="TOutgoingPayloadType"></typeparam>
	public abstract class BaseUnityNetworkClient<TIncomingPayloadType, TOutgoingPayloadType> : MonoBehaviour, INetworkMessageReceivable<TIncomingPayloadType>, IConnectionService
		where TOutgoingPayloadType : class
		where TIncomingPayloadType : class
	{
		//We expose this only because the underlying client MAY need to do something with it. Maybe export it to bring it to another scene.
		/// <summary>
		/// The managed network client that the Unity3D client is implemented on-top of.
		/// </summary>
		protected abstract IManagedNetworkClient<TOutgoingPayloadType, TIncomingPayloadType> Client { get; }

		/// <summary>
		/// The logger for the client.
		/// </summary>
		public abstract ILog Logger { get; }

		/// <summary>
		/// Starts dispatching the messages and won't yield until
		/// the client has stopped or has disconnected.
		/// </summary>
		/// <returns></returns>
		protected async Task StartDispatchingAsync()
		{
			try
			{
				if(!Client.isConnected && Logger.IsWarnEnabled)
					Logger.Warn($"The client {name} was not connected before dispatching started.");

				while(Client.isConnected)
				{
					if(Logger.IsDebugEnabled)
						Logger.Debug("Reading message.");

					//TODO: Add configurable token source
					NetworkIncomingMessage<TIncomingPayloadType> message = await Client.ReadMessageAsync(CancellationToken.None)
						.ConfigureAwait(false);

					//Supress and continue reading
					try
					{
						await OnNetworkMessageRecieved(message)
							.ConfigureAwait(true);
					}
					catch(Exception e)
					{
						if(Logger.IsDebugEnabled)
							Logger.Debug($"Error: {e.Message}\n\n Stack Trace: {e.StackTrace}");
					}

				}
			}
			catch(Exception e)
			{
				if(Logger.IsDebugEnabled)
					Logger.Debug($"Error: {e.Message}\n\n Stack Trace: {e.StackTrace}");

				throw;
			}

			if(Logger.IsDebugEnabled)
				Logger.Debug("Network client stopped reading.");
		}

		/// <inheritdoc />
		public abstract Task OnNetworkMessageRecieved(NetworkIncomingMessage<TIncomingPayloadType> message);

		//TODO: Should these be virtual?
		/// <inheritdoc />
		public Task DisconnectAsync(int delay)
		{
			return Client.DisconnectAsync(delay);
		}

		/// <inheritdoc />
		public async Task<bool> ConnectAsync(string ip, int port)
		{

			bool result = await Client.ConnectAsync(ip, port);

			//TODO: How should we handle multiple connection requests? We may have a dispatching thread going
			if(result)
				Task.Factory.StartNew(StartDispatchingAsync, CancellationToken.None,
					TaskCreationOptions.LongRunning, TaskScheduler.FromCurrentSynchronizationContext());

			return result;
		}

		/// <inheritdoc />
		public bool isConnected => Client.isConnected;
	}
}
