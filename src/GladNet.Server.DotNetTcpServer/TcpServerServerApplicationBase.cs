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
	/// Builds, manages and maintains <see cref="ManagedSession{TPayloadWriteType,TPayloadReadType}"/>s internally
	/// when clients connect.
	/// </summary>
	/// <typeparam name="TPayloadWriteType"></typeparam>
	/// <typeparam name="TPayloadReadType"></typeparam>
	public abstract class TcpServerServerApplicationBase<TPayloadWriteType, TPayloadReadType> 
		: IServerApplicationListenable, IFactoryCreatable<ManagedSession<TPayloadWriteType, TPayloadReadType>, SessionCreationContext>
		where TPayloadWriteType : class 
		where TPayloadReadType : class
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
		protected ConcurrentDictionary<int, ManagedSession<TPayloadWriteType, TPayloadReadType>> Sessions { get; } = new ConcurrentDictionary<int, ManagedSession<TPayloadWriteType, TPayloadReadType>>();

		protected TcpServerServerApplicationBase(NetworkAddressInfo serverAddress, ILog logger)
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

			using (Socket listenSocket = new Socket(SocketType.Stream, ProtocolType.Tcp))
			{
				listenSocket.Bind(new IPEndPoint(ServerAddress.AddressEndpoint, ServerAddress.Port));

				//TODO: This is the maximum unacceptaed in-queue connection attempts. I don't know if this should be configurable
				//See: https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socket.listen?view=netframework-4.7.2
				listenSocket.Listen(1000);

				if(token.IsCancellationRequested)
					return;

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

					//		//connection, new SessionDetails(new NetworkAddressInfo(clientAddress.Address, ServerAddress.Port), connectionId)
					ManagedSession<TPayloadWriteType, TPayloadReadType> clientSession = Create(new SessionCreationContext(connection, new SessionDetails(new NetworkAddressInfo(clientAddress.Address, ServerAddress.Port), connectionId)));

					clientSession.AttachDisposableResource(connection);
					clientSession.AttachDisposableResource(socket);

					StartNetworkSessionTasks(token, clientSession);

					if (!Sessions.TryAdd(connectionId, clientSession))
						throw new InvalidOperationException($"Failed to add Session: {clientSession} to {Sessions} container with Id: {connectionId}");
				}
			}
		}

		private static void StartNetworkSessionTasks(CancellationToken token, ManagedSession<TPayloadWriteType, TPayloadReadType> clientSession)
		{
			CancellationToken sessionCancelToken = new CancellationToken(false);
			CancellationTokenSource combinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, sessionCancelToken);

			//It's important to use the root cancel token, and not the session token, for thread/task cancel because
			//if the combined or session token is cancelled we need to dispose of the source and it's possible it will leak
			//if we don't finish the task. But does Finally ALWAYS run? Even if a Task/Thread never continues? I don't know honestly.
			//But this should be safe either way.
			Task.Run(async () =>
			{
				try
				{
					await clientSession.StartWritingAsync(combinedTokenSource.Token);
				}
				finally
				{
					combinedTokenSource.Dispose();
				}
			}, token);

			Task.Run(async () =>
			{
				try
				{
					await clientSession.StartListeningAsync(combinedTokenSource.Token);
				}
				finally
				{
					combinedTokenSource.Dispose();
				}
			}, token);
		}

		//Should be overriden by the consumer of the library.
		/// <summary>
		/// Called internally when a session is being created.
		/// This method should produce a valid session and is considered the hub of the connection.
		/// </summary>
		/// <param name="context">The context for creating the managed session.</param>
		/// <returns>A non-null session.</returns>
		public abstract ManagedSession<TPayloadWriteType, TPayloadReadType> Create(SessionCreationContext context);
	}
}
