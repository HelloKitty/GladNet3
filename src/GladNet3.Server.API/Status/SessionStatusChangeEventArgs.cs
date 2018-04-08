using System;
using System.Collections.Generic;
using System.Text;

namespace GladNet
{
	public class SessionStatusChangeEventArgs : EventArgs
	{
		/// <summary>
		/// The connection status changed to.
		/// </summary>
		public ConnectionStatus Status { get; }

		/// <summary>
		/// The session details.
		/// </summary>
		public SessionDetails Details { get; }

		public SessionStatusChangeEventArgs(ConnectionStatus status, SessionDetails details)
		{
			if(details == null) throw new ArgumentNullException(nameof(details));
			if(!Enum.IsDefined(typeof(ConnectionStatus), status)) throw new ArgumentOutOfRangeException(nameof(status), "Value should be defined in the ConnectionStatus enum.");

			Status = status;
			Details = details;
		}
	}
}
