using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GladNet
{
	/// <summary>
	/// Delegate type for status changes.
	/// </summary>
	/// <param name="source"></param>
	public delegate Task StatusChangeEvent(IManagedClientSession source, SessionStatusChangeEventArgs args);
}
