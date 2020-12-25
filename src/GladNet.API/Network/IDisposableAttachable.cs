using System;
using System.Collections.Generic;
using System.Text;

namespace GladNet
{
	/// <summary>
	/// Contract for types that can have <see cref="IDisposable"/>s attached to them.
	/// </summary>
	public interface IDisposableAttachable
	{
		/// <summary>
		/// Attaches the disposable instance to the object.
		/// </summary>
		/// <param name="disposable">The disposable to attach.</param>
		void AttachDisposable(IDisposable disposable);
	}
}
