using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	/// <summary>
	/// Represents the result of trying to send a networked message.
	/// </summary>
	public enum SendResult
	{
		//Indicates an invalid send
		Invalid = 0,

		//Indicates the the peer is not connected to the network and failed to send.
		FailedNotConnected = 1,

		//Indicates a successful send
		Sent = 2,

		//Indicates that the message was enqueued.
		Queued = 3,
	}
}
