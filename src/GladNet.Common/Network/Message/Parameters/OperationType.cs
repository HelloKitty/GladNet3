using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	/// <summary>
	/// Represents valid operation types for networked messages.
	/// </summary>
	public enum OperationType
	{
		//Indicates an operation from the result of a peer sending an unsoliticted message.
		Event = 0,

		//Indicates a peer is requesting something from another peer.
		Request = 1,

		//Indicates a peer is responding to another peer about a request.
		Response = 2,
	}
}
