using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Common.Logging.Simple;
using Glader.Essentials;
using GladNet;
using Pipelines.Sockets.Unofficial;

namespace GladNet
{
	/// <summary>
	/// Base application base for a high performance async TCP server application.
	/// Builds, manages and maintains <see cref="ManagedSession"/>s internally
	/// when clients connect.
	/// </summary>
	public abstract class TcpServerApplicationBase<TManagedSessionType>
		: IServerApplicationListenable, IFactoryCreatable<TManagedSessionType, SessionCreationContext>
		where TManagedSessionType : ManagedSession
	{
		/// <summary>
		/// Network address information for the server.
		/// </summary>
		public NetworkAddressInfo ServerAddress { get; }

		/// <summary>
		/// Server application logger.
		/// </summary>
		public ILog Logger { get; }

		private int _lifetimeConnectionCount = 0;

		/// <summary>
		/// The number of connections that have been serviced
		/// lifetime by this application.
		/// </summary>
		public int LifetimeConnectionCount
		{
			get => _lifetimeConnectionCount;
			private set => _lifetimeConnectionCount = value;
		}

		//TODO: We need a better API for exposing this.
		protected ConcurrentDictionary<int, TManagedSessionType> Sessions { get; } = new ConcurrentDictionary<int, TManagedSessionType>();

		/// <summary>
		/// Event that is fired when a managed session is ended.
		/// This could be caused by disconnection but is not required to be related to disconnection.
		/// </summary>
		public event EventHandler<ManagedSessionContextualEventArgs<TManagedSessionType>> OnManagedSessionEnded;

		protected TcpServerApplicationBase(NetworkAddressInfo serverAddress, ILog logger)
		{
			ServerAddress = serverAddress ?? throw new ArgumentNullException(nameof(serverAddress));
			Logger = logger;
		}

		/// <summary>
		/// Indicates if the provided <see cref="Socket"/> is acceptable.
		/// Return true if the client should be handled. This will likely lead to
		/// a call to <see cref="Create"/>. Returning false
		/// will mean this connection should be disconnected and rejected and no
		/// client representation will be created for it.
		/// </summary>
		/// <param name="connection">The <see cref="Socket"/> to check the acceptance for.</param>
		/// <returns></returns>
		protected abstract bool IsClientAcceptable(Socket connection);

		/// <inheritdoc />
		public async Task BeginListeningAsync(CancellationToken token = default)
		{
			SocketConnection.AssertDependencies();

			if (Logger.IsInfoEnabled)
				Logger.Info($"Server begin listening.");

			using (Socket listenSocket = new Socket(SocketType.Stream, ProtocolType.Tcp))
			{
				listenSocket.Bind(new IPEndPoint(ServerAddress.AddressEndpoint, ServerAddress.Port));

				//TODO: This is the maximum unacceptaed in-queue connection attempts. I don't know if this should be configurable
				//See: https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socket.listen?view=netframework-4.7.2
				listenSocket.Listen(1000);

				if(token.IsCancellationRequested)
					return;

				if(Logger.IsInfoEnabled)
					Logger.Info($"Server bound to Port: {ServerAddress.Port} on Address: {ServerAddress.AddressEndpoint.ToString()}");

				while(!token.IsCancellationRequested)
				{
					Socket socket = await listenSocket.AcceptAsync();

					if (!IsClientAcceptable(socket))
					{
						socket.Close();
						continue;
					}

					IPEndPoint clientAddress = ((IPEndPoint) socket.RemoteEndPoint);
					int connectionId = Interlocked.Increment(ref _lifetimeConnectionCount);
					SocketConnection connection = SocketConnection.Create(socket, PipeOptions.Default, PipeOptions.Default, SocketConnectionOptions.ZeroLengthReads);

					if(Logger.IsInfoEnabled)
						Logger.Info($"Attempting to create Session for Address: {clientAddress.Address} Id: {connectionId}");

					TManagedSessionType clientSession = Create(new SessionCreationContext(connection, new SessionDetails(new NetworkAddressInfo(clientAddress.Address, ServerAddress.Port), connectionId)));

					clientSession.AttachDisposableResource(connection);
					clientSession.AttachDisposableResource(socket);

					StartNetworkSessionTasks(token, clientSession);

					if (!Sessions.TryAdd(connectionId, clientSession))
						throw new InvalidOperationException($"Failed to add Session: {clientSession} to {Sessions} container with Id: {connectionId}");
				}
			}
		}

		private void StartNetworkSessionTasks(CancellationToken token, TManagedSessionType clientSession)
		{
			CancellationToken sessionCancelToken = new CancellationToken(false);
			CancellationTokenSource combinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, sessionCancelToken);

			Task writeTask = Task.Run(async () => await StartSessionNetworkThreadAsync(clientSession.Details, clientSession.StartWritingAsync(combinedTokenSource.Token), combinedTokenSource, "Write"), token);
			Task readTask = Task.Run(async () => await StartSessionNetworkThreadAsync(clientSession.Details, clientSession.StartListeningAsync(combinedTokenSource.Token), combinedTokenSource, "Read"), token);

			Task.Run(async () =>
			{
				try
				{
					await Task.WhenAll(readTask, writeTask);
				}
				catch (Exception e)
				{
					//Suppress this exception, we have critical deconstruction code to run.
					if (Logger.IsErrorEnabled)
						Logger.Error($"Session: {clientSession.Details.ConnectionId} encountered critical failure in awaiting network task. Error: {e}");
				}

				if (Logger.IsDebugEnabled)
					Logger.Debug($"Session: {clientSession.Details.ConnectionId} Stopped Network Read/Write.");

				//Fire off to anyone interested in managed session ending. We should do this before we fully dispose it and remove it from the session collection.
				OnManagedSessionEnded?.Invoke(this, new ManagedSessionContextualEventArgs<TManagedSessionType>(clientSession));
				Sessions.TryRemove(clientSession.Details.ConnectionId, out _);

				//Now that it's removed, we should also dispose it.
				try
				{
					clientSession.Dispose();
				}
				catch (Exception e)
				{
					if (Logger.IsErrorEnabled)
						Logger.Error($"Encountered error in Client: {clientSession.Details.ConnectionId} session disposal. Error: {e}");
					throw;
				}

			}, token);
		}

		private async Task StartSessionNetworkThreadAsync(SessionDetails details, Task task, CancellationTokenSource combinedTokenSource, string taskName)
		{
			if (details == null) throw new ArgumentNullException(nameof(details));
			if (task == null) throw new ArgumentNullException(nameof(task));

			try
			{
				await task;
			}
			catch (Exception e)
			{
				if (Logger.IsErrorEnabled)
					Logger.Error($"Session: {details.ConnectionId} encountered error in network {taskName} thread. Error: {e}");
			}
			finally
			{
				//It's important that if we arrive at this point WITHOUT canceling somehow
				//then we should cancel the combined source. Otherwise anything else depending on this
				//token won't actually cancel and it likely SHOULD
				if (!combinedTokenSource.IsCancellationRequested)
					combinedTokenSource.Cancel();

				combinedTokenSource.Dispose();
			}
		}

		//Should be overriden by the consumer of the library.
		/// <summary>
		/// Called internally when a session is being created.
		/// This method should produce a valid session and is considered the hub of the connection.
		/// </summary>
		/// <param name="context">The context for creating the managed session.</param>
		/// <returns>A non-null session.</returns>
		public abstract TManagedSessionType Create(SessionCreationContext context);
	}
}
