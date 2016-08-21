using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GladNet.Lidgren.Engine.Common
{
	public class SemaphoreQueue<T> : Queue<T>, IThreadedQueue<Semaphore>
	{
		/// <summary>
		/// A manual-managed semaphore for the <see cref="Queue{T}"/>.
		/// </summary>
		public Semaphore QueueSemaphore { get; } = new Semaphore(0, int.MaxValue);

		/// <summary>
		/// A read/write optimized syncronization queue.
		/// </summary>
		public ReaderWriterLockSlim syncRoot { get; } = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion); //Unity requires no recursion

		/// <summary>
		/// Creates a default <see cref="WaitableQueue{T}"/> with a <see cref="EventWaitHandle"/>.
		/// </summary>
		public SemaphoreQueue()
		{

		}

		//We do debugging to check for proper syncronization
#if DEBUG || DEBUGBUILD
		public new void Enqueue(T item)
		{
			//We could get false positives, in the opposite sense, but it's better than no debug protection
			if (!syncRoot.IsWriteLockHeld && !syncRoot.IsReadLockHeld)
				throw new InvalidOperationException($"Cannot {nameof(Enqueue)} objects in {nameof(WaitableQueue<T>)} without locking with {nameof(syncRoot)}.");

			base.Enqueue(item);
		}
#endif

		//We do debugging to check for proper syncronization
#if DEBUG || DEBUGBUILD
		public new T Dequeue()
		{
			//We could get false positives, in the opposite sense, but it's better than no debug protection
			if (!syncRoot.IsWriteLockHeld && !syncRoot.IsReadLockHeld)
				throw new InvalidOperationException($"Cannot {nameof(Dequeue)} objects in {nameof(WaitableQueue<T>)} without locking with {nameof(syncRoot)}.");

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

				this.syncRoot.Dispose();
				this.QueueSemaphore.Close();

				disposedValue = true;
			}
		}

		 ~SemaphoreQueue()
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
