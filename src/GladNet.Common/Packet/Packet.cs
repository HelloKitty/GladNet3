using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	public class Packet
	{
		/// <summary>
		/// Represents valid operation types for networked messages.
		/// </summary>
		public enum OperationType : byte
		{
			Event = 0,
			Request = 1,
			Response = 2
		}

		/// <summary>
		/// Represents the potential delivery methods for packets to be sent.
		/// </summary>
		public enum DeliveryMethod
		{
			UnreliableAcceptDuplicate,
			UnreliableDiscardStale,
			ReliableUnordered,
			ReliableDiscardStale,
			ReliableOrdered
		}

		/// <summary>
		/// Represents the result of trying to send a networked message.
		/// </summary>
		public enum SendResult : byte
		{
			FailedNotConnected = 0,
			Sent = 1,
			Queued = 2,
			Dropped = 3,
		}
	}
}
