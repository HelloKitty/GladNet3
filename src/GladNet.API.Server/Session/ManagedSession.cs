using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GladNet;

namespace GladNet
{
	/// <summary>
	/// Base type for managed sessions.
	/// </summary>
	public abstract class ManagedSession : IManagedSession, IDisposable
	{
		/// <summary>
		/// Internal lock object.
		/// </summary>
		protected readonly object SyncObj = new object();

		/// <summary>
		/// Represents all the disposable dependencies of a <see cref="ManagedSession"/>.
		/// This will be disposed when the session is disposed.
		/// </summary>
		private List<IDisposable> InternalDisposables { get; } = new List<IDisposable>();

		/// <inheritdoc />
		public IConnectionService ConnectionService { get; }

		/// <inheritdoc />
		public SessionDetails Details { get; }

		/// <summary>
		/// Represents the network configuration options for the session.
		/// </summary>
		protected NetworkConnectionOptions NetworkOptions { get; }

		protected ManagedSession(IConnectionService connectionService, SessionDetails details, NetworkConnectionOptions networkOptions)
		{
			ConnectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
			Details = details ?? throw new ArgumentNullException(nameof(details));
			NetworkOptions = networkOptions ?? throw new ArgumentNullException(nameof(networkOptions));
		}

		/// <summary>
		/// Implementers should start the internal network listening async algorithm.
		/// </summary>
		/// <returns>Awaitable that completes when listening is finished.</returns>
		public abstract Task StartListeningAsync(CancellationToken token = default);

		/// <summary>
		/// Implementers should start the internal network writing async algorithm.
		/// </summary>
		/// <returns>Awaitable that completes when writing is finished.</returns>
		public abstract Task StartWritingAsync(CancellationToken token = default);

		public void Dispose()
		{
			lock(SyncObj)
				foreach (var disposable in InternalDisposables)
					disposable.Dispose();
		}

		/// <summary>
		/// Attaches a disposable <see cref="IDisposable"/> dependency/resource to the <see cref="ManagedSession"/>.
		/// This will be disposed of alongside when <see cref="ManagedSession"/>'s <see cref="Dispose"/> is called.
		/// </summary>
		/// <param name="disposable"></param>
		public void AttachDisposableResource(IDisposable disposable)
		{
			if (disposable == null) throw new ArgumentNullException(nameof(disposable));

			lock(SyncObj)
				InternalDisposables.Add(disposable);
		}
	}
}
