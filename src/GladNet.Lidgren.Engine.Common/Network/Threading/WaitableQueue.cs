using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GladNet.Lidgren.Engine.Common
{
	public class WaitableQueue<T> : Queue<T>, IThreadedQueue<T, EventWaitHandle>
	{
		/// <summary>
		/// A manual-managed semaphore for the <see cref="Queue{T}"/>.
		/// </summary>
		public EventWaitHandle QueueSemaphore { get; } = new EventWaitHandle(false, EventResetMode.ManualReset);

		/// <summary>
		/// A read/write optimized syncronization queue.
		/// </summary>
		public ReaderWriterLockSlim SyncRoot { get; } = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion); //Unity requires no recursion

		WaitHandle IThreadedQueue<T>.QueueSemaphore { get { return QueueSemaphore; } }

		/// <summary>
		/// Creates a default <see cref="WaitableQueue{T}"/> with a <see cref="EventWaitHandle"/>.
		/// </summary>
		public WaitableQueue()
		{

		}

		public IEnumerable<T> DequeueAll()
		{
#if DEBUG || DEBUGBUILD
			if (!SyncRoot.IsWriteLockHeld && !SyncRoot.IsReadLockHeld)
				throw new InvalidOperationException($"Cannot {nameof(DequeueAll)} objects in {nameof(WaitableQueue<T>)} without locking with {nameof(SyncRoot)}.");
#endif

			IEnumerable<T> dequeued = this.ToList();
			this.Clear();
			return dequeued;
		}

		//We do debugging to check for proper syncronization
#if DEBUG || DEBUGBUILD
		public new void Enqueue(T item)
		{
			//We could get false positives, in the opposite sense, but it's better than no debug protection
			if (!SyncRoot.IsWriteLockHeld && !SyncRoot.IsReadLockHeld)
				throw new InvalidOperationException($"Cannot {nameof(Enqueue)} objects in {nameof(WaitableQueue<T>)} without locking with {nameof(SyncRoot)}.");

			base.Enqueue(item);
		}
#endif

		//We do debugging to check for proper syncronization
#if DEBUG || DEBUGBUILD
		public new T Dequeue()
		{
			//We could get false positives, in the opposite sense, but it's better than no debug protection
			if (!SyncRoot.IsWriteLockHeld && !SyncRoot.IsReadLockHeld)
				throw new InvalidOperationException($"Cannot {nameof(Dequeue)} objects in {nameof(WaitableQueue<T>)} without locking with {nameof(SyncRoot)}.");

			return base.Dequeue();
		}
#endif

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
					this.Clear();
				}

				this.SyncRoot.Dispose();
				this.QueueSemaphore.Close();

				disposedValue = true;
			}
		}

		~WaitableQueue()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(false);
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);

			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
