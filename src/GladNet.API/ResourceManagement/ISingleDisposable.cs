using System;
using System.Collections.Generic;
using System.Text;

namespace GladNet
{
	/// <summary>
	/// Contract for <see cref="IDisposable"/>s that can be disposed a single time.
	/// </summary>
	public interface ISingleDisposable : IDisposable
	{
		/// <summary>
		/// Indicates if the <see cref="IDisposable"/> has been disposed.
		/// </summary>
		bool isDisposed { get; }
	}
}
