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
	/// <typeparam name="TPayloadWriteType"></typeparam>
	/// <typeparam name="TPayloadReadType"></typeparam>
	public abstract class ManagedSession<TPayloadWriteType, TPayloadReadType> : IManagedSession, IDisposable
		where TPayloadWriteType : class 
		where TPayloadReadType : class
	{
		/// <summary>
		/// Internal lock object.
		/// </summary>
		protected readonly object SyncObj = new object();

		/// <summary>
		/// Represents all the disposable dependencies of a <see cref="ManagedSession{TPayloadWriteType,TPayloadReadType}"/>.
		/// This will be disposed when the session is disposed.
		/// </summary>
		private List<IDisposable> InternalDisposables { get; } = new List<IDisposable>();

		/// <inheritdoc />
		public IConnectionService ConnectionService { get; }

		/// <inheritdoc />
		public SessionDetails Details { get; }

		protected ManagedSession(IConnectionService connectionService, SessionDetails details)
		{
			ConnectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
			Details = details ?? throw new ArgumentNullException(nameof(details));
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
		/// Attaches a disposable <see cref="IDisposable"/> dependency/resource to the <see cref="ManagedSession{TPayloadWriteType,TPayloadReadType}"/>.
		/// This will be disposed of alongside when <see cref="ManagedSession{TPayloadWriteType,TPayloadReadType}"/>'s <see cref="Dispose"/> is called.
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
