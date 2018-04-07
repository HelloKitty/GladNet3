using System;
using System.Collections.Generic;
using System.Text;

namespace GladNet
{
	/// <summary>
	/// Contains information about the session.
	/// </summary>
	public sealed class SessionDetails
	{
		/// <summary>
		/// Address for the session.
		/// </summary>
		public NetworkAddressInfo Address { get; }

		/// <summary>
		/// The unique connection identifier.
		/// </summary>
		public int ConnectionId { get; }

		/// <inheritdoc />
		public SessionDetails(NetworkAddressInfo address, int connectionId)
		{
			if(address == null) throw new ArgumentNullException(nameof(address));
			if(connectionId < 0) throw new ArgumentOutOfRangeException(nameof(connectionId));

			Address = address;
			ConnectionId = connectionId;
		}
	}
}
