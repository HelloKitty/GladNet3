using System;
using System.Collections.Generic;
using System.Text;

namespace GladNet
{
	public sealed class DisconnectedSessionStatusChangeEventArgs : SessionStatusChangeEventArgs
	{
		/// <inheritdoc />
		public DisconnectedSessionStatusChangeEventArgs(SessionDetails details) 
			: base(ConnectionStatus.Disconnected, details)
		{

		}
	}
}
