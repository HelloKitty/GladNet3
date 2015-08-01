using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GladNet.Common
{
	/// <summary>
	/// Abstract type of all networked messages. Expects inheritors to implement dispatch functionality.
	/// Contains various network message related Enums.
	/// </summary>
	public abstract class NetworkMessage : INetworkMessage, IShallowCloneable<NetworkMessage>
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

		/// <summary>
		/// Represents the potential delivery methods for packets to be sent.
		/// </summary>
		public enum DeliveryMethod
		{
			//Following comments are based on https://code.google.com/p/lidgren-network-gen3/wiki/Basics and https://github.com/lidgren/lidgren-network-gen3/blob/master/Lidgren.Network/NetDeliveryMethod.cs

			//Represents an unknown delivery method.
			Unknown,

			//This is just UDP. Messages can be lost or received more than once. Messages may not be received in the same order as they were sent.
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

		/// <summary>
		/// The payload of a <see cref="INetworkMessage"/>. Can be sent accross a network.
		/// <see cref="NetSendable"/> enforces its wire readyness.
		/// </summary>
		public NetSendable<PacketPayload> Payload { get; private set; }

		/// <summary>
		/// Main constructor for <see cref="NetworkMessage"/> that requires a <see cref="PacketPayload"/> payload.
		/// </summary>
		/// <exception cref="ArgumentNullException">Throws if <see cref="PacketPayload"/> instance supplied is null.</exception>
		/// <param name="payload">The <see cref="PacketPayload"/> of the message.</param>
		protected NetworkMessage(PacketPayload payload)
		{
			if (payload == null)
				throw new ArgumentNullException("payload", "A null Packet cannot be sent accross the network. Please supply at least default.");

			Payload = new NetSendable<PacketPayload>(payload);
		}

		/// <summary>
		/// Method dispatches a substype of <see cref="NetworkMessage"/> to the proper method on an <see cref="INetworkMessageReceiver"/>
		/// Inheriting classes must implement this and target the proper method of to dispatch.
		/// </summary>
		/// <param name="receiver">The target for the subtype <see cref="NetworkMessage"/>.</param>
		/// <param name="parameters">The parameters with which the message was sent.</param>
		public abstract void Dispatch(INetworkMessageReceiver receiver, IMessageParameters parameters);

		public virtual NetworkMessage ShallowClone()
		{
			//As of Oct. 8th it is valid to call a MemberwiseCLone to generate a shallow copy.
			return MemberwiseClone() as NetworkMessage;
		}

		object IShallowCloneable.ShallowClone()
		{
			return this.ShallowClone();
		}
	}
}
