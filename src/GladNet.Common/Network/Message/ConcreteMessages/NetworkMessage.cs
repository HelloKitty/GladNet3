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

		//TODO: Prevent routing exploits. Right now clients could spoof routing info
		//WE NEED TO SANITIZE
#if !ENDUSER
		/// <summary>
		/// Indicates if the message has any valid keys for routing.
		/// </summary>
		public bool isMessageRoutable
		{
			get
			{
				//Have to lock
				lock(syncObj)
				{
					return _routingCodeStack != null && _routingCodeStack.Count != 0;
				}
			}
		}


		//We should manage this structure internally as there is no reason to expose it
		//In fact, it may change implementation in the future and is something depend on it would be
		//a disaster similar to the one we're in right now implementing this feature lol
		/// <summary>
		/// Internally managed wire-ready routing code stack.
		/// This carries critical information about how a message should be routed through the server.
		/// </summary>
		[GladNetMember(GladNetDataIndex.Index6)]
		private Stack<int> _routingCodeStack = null;
#endif

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

#if !ENDUSER
		/// <summary>
		/// Pushes a new routing key into the message.
		/// This key indicates where a message to this message should be routed back to.
		/// </summary>
		/// <param name="routingKey">Unique routing key.</param>
		public void Push(int routingKey)
		{
			lock(syncObj)
			{
				if (_routingCodeStack == null)
					_routingCodeStack = new Stack<int>(1); //most will only need a depth of 2 routing so only 1 slot is needed

				_routingCodeStack.Push(routingKey);
			}
		}

		/// <summary>
		/// Removes a routing key from the message.
		/// This key indicates where this message should be forwared to.
		/// </summary>
		/// <returns>A unique routing key.</returns>
		public int? Pop()
		{
			lock(syncObj)
			{
				if (_routingCodeStack == null)
					return null;
				else
					if (_routingCodeStack.Count != 0)
						return _routingCodeStack.Pop();
					else
						return null;
			}
		}

		/// <summary>
		/// Peeks at the routing key this message would use
		/// to route. Call Pop to both Peek and Remove the key before sending.
		/// </summary>
		/// <returns>Returns the routing ID or null if there are no routing IDs.</returns>
		public int? Peek()
		{
			lock (syncObj)
			{
				if (_routingCodeStack == null)
					return null;
				else
					if (_routingCodeStack.Count != 0)
						return _routingCodeStack.Peek();
					else
						return null;
			}
		}

		public void ExportRoutingDataTo(IRoutableMessage message)
		{
			//So, this is sorta a hack but it's a good one
			//for preformance
			if(message is NetworkMessage)
			{
				NetworkMessage castedMessage = message as NetworkMessage;
				lock(syncObj)
				{
					//No reason to copy null stack
					if(_routingCodeStack != null)
						//We should transfer the routing stack but also preserve the other routing stack
						//We probably won't need it but just in case the user wants to do something with it still
						castedMessage._routingCodeStack = new Stack<int>(_routingCodeStack.Reverse()); //We must create a reverse copy of the stack:http://stackoverflow.com/questions/7391348/c-sharp-clone-a-stack
				}
			}
			
			//TOOD: Implement the routing for non-NetworkMessages
		}
#endif
	}
}
