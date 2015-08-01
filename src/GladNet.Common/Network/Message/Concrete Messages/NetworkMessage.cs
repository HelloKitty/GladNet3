using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	public abstract class NetworkMessage
	{
		/// <summary>
		/// Represents valid operation types for networked messages.
		/// </summary>
		public enum OperationType : byte
		{
			//Indicates an operation from the result of a peer sending an unsoliticted message.
			Event = 0,

			//Indicates a peer is requesting something from another peer.
			Request = 1,

			//Indicates a peer is responding to another peer about a request.
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

		/// <summary>
		/// A <see cref="PacketPayload"/> instance that contains the high level payload of the <see cref="NetworkMessage"/>
		/// </summary>
		public PacketPayload PacketData { get; private set; }

		/// <summary>
		/// Main constructor for <see cref="NetworkMessage"/> that requires a <see cref="PacketPayload"/> payload.
		/// </summary>
		/// <exception cref="ArgumentNullException">Throws if <see cref="PacketPayload"/> instance supplied is null.</exception>
		/// <param name="p"></param>
		public NetworkMessage(PacketPayload packetForMessage)
		{
			if (packetForMessage == null)
				throw new ArgumentNullException("packetForMessage", "A null Packet cannot be sent accross the network. Please supply at least default.");
				
			PacketData = packetForMessage;
		}

		/// <summary>
		/// Method dispatches a substype of <see cref="NetworkMessage"/> to the proper method on an <see cref="INetworkMessageReceiver"/>
		/// </summary>
		/// <param name="receiver">The target for the subtype <see cref="NetworkMessage"/>.</param>
		public abstract void Dispatch(INetworkMessageReceiver receiver, IMessageParameters mParams);
	}
}
