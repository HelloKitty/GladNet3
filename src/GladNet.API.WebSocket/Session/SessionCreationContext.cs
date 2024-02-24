using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;

namespace GladNet
{
	public sealed class SessionCreationContext
	{
		/// <summary>
		/// The underlying connection object for the session.
		/// </summary>
		public WebSocket Connection { get; }

		/// <summary>
		/// The details of the session.
		/// </summary>
		public SessionDetails Details { get; }

		public SessionCreationContext(WebSocket connection, SessionDetails details)
		{
			Connection = connection ?? throw new ArgumentNullException(nameof(connection));
			Details = details ?? throw new ArgumentNullException(nameof(details));
		}
	}
}
