using System;
using System.Collections.Generic;
using System.Text;

namespace GladNet
{
	/// <summary>
	/// Delegate type for status changes.
	/// </summary>
	/// <param name="source"></param>
	public delegate void StatusChangeEvent(IManagedClientSession source, SessionStatusChangeEventArgs args);
}
