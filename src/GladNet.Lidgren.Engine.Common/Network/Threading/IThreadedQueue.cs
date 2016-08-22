using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GladNet.Lidgren.Engine.Common
{
	public interface IThreadedQueue<T, TWaitHandleType> : IThreadedQueue<T>, IDisposable
		where TWaitHandleType : WaitHandle
	{
		new TWaitHandleType QueueSemaphore { get; }
	}

	public interface IThreadedQueue<T> : IDisposable
	{
		WaitHandle QueueSemaphore { get; }

		ReaderWriterLockSlim syncRoot { get; }

		T Dequeue();

		IEnumerable<T> DequeueAll();
	}
}
