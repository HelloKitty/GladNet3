using Easyception;
using GladNet.Common;
using GladNet.Payload;
using GladNet.Serializer;
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
	[GladNetSerializationContract]
	[GladNetSerializationInclude(1, typeof(EventMessage))]
	[GladNetSerializationInclude(2, typeof(ResponseMessage))]
	[GladNetSerializationInclude(3, typeof(RequestMessage))]
	[GladNetSerializationInclude(4, typeof(StatusMessage))]
	public abstract class NetworkMessage : INetworkMessage, IDeepCloneable<NetworkMessage>
	{
		/// <summary>
		/// Internal class locking/sync object.
		/// </summary>
		protected readonly object syncObj = new object();

		//TODO: Make this NetSendable thread-safe and using proper locking in this class to prevent corruption
		/// <summary>
		/// The payload of a <see cref="INetworkMessage"/>. Can be sent accross a network.
		/// <see cref="NetSendable"/> enforces its wire readyness.
		/// </summary>
		[GladNetMember(GladNetDataIndex.Index5, IsRequired = true)]
		public NetSendable<PacketPayload> Payload { get; private set; }

		//We should manage this structure internally as there is no reason to expose it
		//In fact, it may change implementation in the future and is something depend on it would be
		//a disaster similar to the one we're in right now implementing this feature lol
		/// <summary>
		/// Internally managed wire-ready routing code stack.
		/// This carries critical information about how a message should be routed through the server.
		/// </summary>
		[GladNetMember(GladNetDataIndex.Index6)]
		private Stack<int> _routingCodeStack = null;

		/// <summary>
		/// Main constructor for <see cref="NetworkMessage"/> that requires a <see cref="PacketPayload"/> payload.
		/// </summary>
		/// <exception cref="ArgumentNullException">Throws if <see cref="PacketPayload"/> instance supplied is null.</exception>
		/// <param name="payload">The <see cref="PacketPayload"/> of the message.</param>
		protected NetworkMessage(PacketPayload payload)
			: this(new NetSendable<PacketPayload>(payload))
		{
			//NetSendable should verify non-null payload.
		}

		protected NetworkMessage(NetSendable<PacketPayload> sendPayload)
		{
			Throw<ArgumentNullException>.If.IsNull(sendPayload)
				?.Now(nameof(sendPayload), $"A null {nameof(NetSendable<PacketPayload>)} was passed for {nameof(NetworkMessage)} creation.");

			Payload = sendPayload;
		}

		/// <summary>
		/// Method dispatches a substype of <see cref="NetworkMessage"/> to the proper method on an <see cref="INetworkMessageReceiver"/>
		/// Inheriting classes must implement this and target the proper method of to dispatch.
		/// </summary>
		/// <param name="receiver">The target for the subtype <see cref="NetworkMessage"/>.</param>
		/// <param name="parameters">The parameters with which the message was sent.</param>
		public abstract void Dispatch(INetworkMessageReceiver receiver, IMessageParameters parameters);

		public abstract NetworkMessage DeepClone();

		object IDeepCloneable.DeepClone()
		{
			//Have to lock on both
			lock(syncObj)
				lock(Payload.syncObj)
					return this.DeepClone();
		}

		/// <summary>
		/// Serializes the <see cref="NetworkMessage"/> using a visiting serializer.
		/// The reason we have this is so we can get concrete Type when serializing.
		/// Deserialization is easy because we know what to expect.
		/// </summary>
		/// <param name="serializer"></param>
		/// <returns></returns>
		byte[] ISerializationVisitable.SerializeWithVisitor(ISerializerStrategy serializer)
		{
			//Double check locking
			lock(syncObj)
			{
				//We should lock this second because nobody will ever be locking on THIS classes syncObj
				//and the payload syncObj should never be locked up in deadlock because it's called externally and will eventually end without
				//waiting for this classes lock
				lock(Payload.syncObj)
				{
					//once inside this lock we need to check the Payload's state
					//I don't really know if this is how we should handle this but I guess it works
					if (Payload.DataState == NetSendableState.Default)
						Payload.Serialize(serializer);

					//We need visit style functionality because some serializers require to be told about the class in the heirarhcy
					//For example protobuf-net won't accept interface serialization with types.
					return serializer.Serialize<NetworkMessage>(this);
				}
			}
		}
	}
}
