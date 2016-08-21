using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GladNet.Lidgren.Engine.Common
{
	public interface IThreadedQueue<TWaitHandleType> : IDisposable
		where TWaitHandleType : WaitHandle
	{
		TWaitHandleType QueueSemaphore { get; }

		ReaderWriterLockSlim syncRoot { get; }
	}
}
