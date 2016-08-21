using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GladNet.Lidgren.Engine.Common
{
	public interface IThreadedQueue<TWaitHandleType>
		where TWaitHandleType : WaitHandle
	{
		TWaitHandleType QueueSemaphore { get; }

		ReaderWriterLockSlim syncRoot { get; }
	}
}
