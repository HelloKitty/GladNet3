using Easyception;
using GladNet.Common;
using GladNet.Payload;
using GladNet.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GladNet.Message
{
	/// <summary>
	/// <see cref="ResponseMessage"/>s are <see cref="NetworkMessage"/>s in response to <see cref="RequestMessage"/> from remote peers.
	/// It contains additional fields/properties compared to <see cref="NetworkMessage"/> that provide information on the response.
	/// </summary>
	[GladNetSerializationContract]
	public class ResponseMessage : NetworkMessage, IResponseMessage
	{
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
				lock (syncObj)
				{
					return _routingCodeStack != null && _routingCodeStack.Count != 0;
				}
			}
		}

		//Read Route-back Outside Userspace for information on why we do this: https://github.com/HelloKitty/GladNet2.Specifications/blob/master/Routing/RoutingSpecification.md
		/// <summary>
		/// Indicates if the message is currently routing back.
		/// This can help indicate to GladNet2 internals whether we should let
		/// the message even reach userspace.
		/// </summary>
		[GladNetMember(GladNetDataIndex.Index1, IsRequired = true)]
		public bool isRoutingBack { get; set; }

		//We should manage this structure internally as there is no reason to expose it
		//In fact, it may change implementation in the future and is something depend on it would be
		//a disaster similar to the one we're in right now implementing this feature lol
		/// <summary>
		/// Internally managed wire-ready routing code stack.
		/// This carries critical information about how a message should be routed through the server.
		/// </summary>
		[GladNetMember(GladNetDataIndex.Index2)]
		internal Stack<int> _routingCodeStack = null;
#endif

		/// <summary>
		/// Constructor for <see cref="ResponseMessage"/> that calls <see cref="NetworkMessage"/>.ctor
		/// </summary>
		/// <param name="payload"><see cref="PacketPayload"/> of the <see cref="NetworkMessage"/>.</param>
		public ResponseMessage(PacketPayload payload)
			: base(payload)
		{
			Throw<ArgumentNullException>.If.IsNull(payload)
				?.Now(nameof(payload), $"Payload for {this.GetType().Name} must not be null in construction.");
		}

		/// <summary>
		/// Protected instructor used for deep cloning the NetworkMessage.
		/// </summary>
		/// <param name="netSendablePayload">Shallow copy of the PacketPayload for copying.</param>
		protected ResponseMessage(NetSendable<PacketPayload> netSendablePayload)
			: base(netSendablePayload)
		{
			//Used for deep cloning
		}

		/// <summary>
		/// Dispatches the <see cref="ResponseMessage"/> (this) to the supplied <see cref="INetworkMessageReceiver"/>.
		/// </summary>
		/// <param name="receiver">The target <see cref="INetworkMessageReceiver"/>.</param>
		/// <exception cref="ArgumentNullException">Throws if either parameters are null.</exception>
		/// <param name="parameters">The <see cref="IMessageParameters"/> of the <see cref="ResponseMessage"/>.</param>
		public override void Dispatch(INetworkMessageReceiver receiver, IMessageParameters parameters)
		{
			Throw<ArgumentNullException>.If.IsNull(receiver)
				?.Now(nameof(receiver), $"{nameof(INetworkMessageReceiver)} parameter is null in {this.GetType().Name}");

			Throw<ArgumentNullException>.If.IsNull(parameters)
				?.Now(nameof(parameters), $"{nameof(IMessageParameters)} parameter is null in {this.GetType().Name}");

			receiver.OnNetworkMessageReceive(this, parameters);
		}

		public override NetworkMessage DeepClone()
		{
			lock (syncObj)
				lock (Payload.syncObj)
					return new ResponseMessage(Payload.ShallowClone());
		}

#if !ENDUSER
		/// <summary>
		/// Pushes a new routing key into the message.
		/// This key indicates where a message to this message should be routed back to.
		/// </summary>
		/// <param name="routingKey">Unique routing key.</param>
		public void Push(int routingKey)
		{
			lock (syncObj)
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
			lock (syncObj)
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
			if (message is RequestMessage)
			{
				RequestMessage castedMessage = message as RequestMessage;
				lock (syncObj)
				{
					//No reason to copy null stack
					if (_routingCodeStack != null)
						//We should transfer the routing stack but also preserve the other routing stack
						//We probably won't need it but just in case the user wants to do something with it still
						castedMessage._routingCodeStack = new Stack<int>(_routingCodeStack.Reverse()); //We must create a reverse copy of the stack:http://stackoverflow.com/questions/7391348/c-sharp-clone-a-stack
				}
			}
			else
			{
				if (message is ResponseMessage)
				{
					ResponseMessage castedMessage = message as ResponseMessage;
					lock (syncObj)
					{
						//No reason to copy null stack
						if (_routingCodeStack != null)
							//We should transfer the routing stack but also preserve the other routing stack
							//We probably won't need it but just in case the user wants to do something with it still
							castedMessage._routingCodeStack = new Stack<int>(_routingCodeStack.Reverse()); //We must create a reverse copy of the stack:http://stackoverflow.com/questions/7391348/c-sharp-clone-a-stack
					}
				}
			}

		}
#endif
	}
}
