using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GladNet.Lidgren.Engine.Common
{
	public class WaitableQueue<T> : Queue<T>
	{
		/// <summary>
		/// A manual-managed semaphore for the <see cref="Queue{T}"/>.
		/// </summary>
		public WaitHandle QueueSemaphore { get; } = new EventWaitHandle(false, EventResetMode.ManualReset);

		/// <summary>
		/// A read/write optimized syncronization queue.
		/// </summary>
		public ReaderWriterLockSlim syncRoot { get; } = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion); //Unity requires no recursion

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
	}
}
