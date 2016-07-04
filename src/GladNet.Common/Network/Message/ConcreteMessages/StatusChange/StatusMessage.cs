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
	[GladNetSerializationContract]
	public class StatusMessage : NetworkMessage, IStatusMessage
	{
		private NetStatus? _Status = null;
		/// <summary>
		/// Indicates the <see cref="NetStatus"/> sent with this <see cref="NetworkMessage"/>.
		/// </summary>
		public NetStatus Status
		{
			get
			{
				//Double check locking
				if(!_Status.HasValue)
					lock(syncObj)
						lock(Payload.syncObj)
							//Constructor enforces the Type. Casting is safe.
							return _Status.HasValue ? _Status.Value : (_Status = (Payload.Data as StatusChangePayload).Status).Value;
				else
					return _Status.Value;
			}
		}

		/// <summary>
		/// Constructor for <see cref="StatusMessage"/> that requires a <see cref="StatusChangePayload"/> payload.
		/// </summary>
		/// <exception cref="ArgumentNullException">Throws if <see cref="StatusChangePayload"/> instance supplied is null.</exception>
		/// <param name="payload">The <see cref="StatusChangePayload"/> of the message.</param>
		public StatusMessage(StatusChangePayload payload)
			: base(payload)
		{

			//This ensures a non-malicious sender sends a StatusChangePayload but it must still be checked.
		}

		/// <summary>
		/// Protected instructor used for deep cloning the NetworkMessage.
		/// </summary>
		/// <param name="netSendablePayload">Shallow copy of the PacketPayload for copying.</param>
		protected StatusMessage(NetSendable<PacketPayload> netSendablePayload)
			: base(netSendablePayload)
		{
			//Used for deep cloning
		}

		/// <summary>
		/// Dispatches the <see cref="StatusMessage"/> (this) to the supplied <see cref="INetworkMessageReceiver"/>.
		/// </summary>
		/// <param name="receiver">The target <see cref="INetworkMessageReceiver"/>.</param>
		/// <exception cref="ArgumentNullException">Throws if the receiver is null.</exception>
		/// <param name="parameters">The <see cref="IMessageParameters"/> of the <see cref="EventMessage"/>.</param>
		public override void Dispatch(INetworkMessageReceiver receiver, IMessageParameters parameters = null)
		{
			//We don't need IMessageParameters for this type of message.
			Throw<ArgumentNullException>.If.IsNull(receiver)
				?.Now(nameof(receiver), $"{nameof(INetworkMessageReceiver)} parameter is null in {this.GetType().Name}");

			receiver.OnNetworkMessageReceive(this, parameters);
		}

		public override NetworkMessage DeepClone()
		{
			lock (syncObj)
				lock (Payload.syncObj)
					//Shallow clone of the payload is valid because internally it's represented as a non-instance specific mutable PacketPayload and/or
					//a non-mutating byte[] that is used for serialization/encryption and new instances of the byte[] are created if they're mutated.
					//This helps us out in preformance when we want to serialize once and encrypt for many.
					return new StatusMessage(Payload.ShallowClone());
		}
	}
}
