using System;
using System.Collections.Generic;
using System.Text;
using Nito.AsyncEx;

namespace GladNet
{
	/// <summary>
	/// Combined buffer and locking object for syncronization.
	/// </summary>
	public sealed class AsyncLockableBuffer
	{
		/// <summary>
		/// The async syncronization object.
		/// </summary>
		public AsyncLock BufferLock { get; } = new AsyncLock();

		/// <summary>
		/// The buffer.
		/// </summary>
		public byte[] Buffer { get; }

		public AsyncLockableBuffer(int size)
		{
			if(size <= 0) throw new ArgumentOutOfRangeException(nameof(size));

			Buffer = new byte[size];
		}
	}
}
