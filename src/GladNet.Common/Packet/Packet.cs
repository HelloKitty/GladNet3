using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	/// <summary>
	/// Represents the GladNet smallest unit of networked information.
	/// </summary>
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
			//Following comments are based on https://code.google.com/p/lidgren-network-gen3/wiki/Basics and https://github.com/lidgren/lidgren-network-gen3/blob/master/Lidgren.Network/NetDeliveryMethod.cs

			//his is just UDP. Messages can be lost or received more than once. Messages may not be received in the same order as they were sent.
			UnreliableAcceptDuplicate,

		
			//Using this delivery method messages can still be lost; but you're protected against duplicated messages. 
			//If a message arrives late; that is, if a message sent after this one has already been received - it will be dropped. 
			//This means you will never receive "older" data than what you already have received.
			UnreliableDiscardStale,

			//This delivery method ensures that every message sent will be eventually received. 
			//It does not however guarantee what order they will be received in; late messages may be delivered before newer ones.
			ReliableUnordered,

			//This delivery method is similar to UnreliableSequenced; except that it guarantees that SOME messages will be received - if you only send one message - it will be received. If you sent two messages quickly, and they get reordered in transit, only the newest message will be received 
			//- but at least ONE of them will be received.
			ReliableDiscardStale,

			//This delivery method guarantees that messages will always be received in the exact order they were sent.
			ReliableOrdered
		}

		/// <summary>
		/// Represents the result of trying to send a networked message.
		/// </summary>
		public enum SendResult : byte
		{
			//Indicates the the peer is not connected to the network and failed to send.
			FailedNotConnected = 0,
			
			//Indicates a successful send
			Sent = 1,

			//Indicates that the message was enqueued.
			Queued = 2,
		}
	}
}
