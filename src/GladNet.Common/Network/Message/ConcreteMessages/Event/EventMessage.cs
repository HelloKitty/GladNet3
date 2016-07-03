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
	/// <see cref="EventMessage"/>s are <see cref="NetworkMessage"/>s that are unexpected responses to network events.
	/// Generally these are messages that are the result of other-client/server state changes that require 'unexpected' network messages
	/// to sync.
	/// </summary>
	[GladNetSerializationContract]
	public class EventMessage : NetworkMessage, IEventMessage
	{
		/// <summary>
		/// Constructor for <see cref="EventMessage"/> that calls <see cref="NetworkMessage"/>.ctor
		/// </summary>
		/// <param name="payload"><see cref="PacketPayload"/> of the <see cref="NetworkMessage"/>.</param>
		public EventMessage(PacketPayload payload)
			: base(payload)
		{
			//TODO: Add exception for null packet.
		}

		/// <summary>
		/// Protected instructor used for deep cloning the NetworkMessage.
		/// </summary>
		/// <param name="netSendablePayload">Shallow copy of the PacketPayload for copying.</param>
		protected EventMessage(NetSendable<PacketPayload> netSendablePayload)
			: base(netSendablePayload)
		{
			//Used for deep cloning
		}

		/// <summary>
		/// Dispatches the <see cref="EventMessage"/> (this) to the supplied <see cref="INetworkMessageReceiver"/>.
		/// </summary>
		/// <param name="receiver">The target <see cref="INetworkMessageReceiver"/>.</param>
		/// <exception cref="ArgumentNullException">Throws if either parameters are null.</exception>
		/// <param name="parameters">The <see cref="IMessageParameters"/> of the <see cref="EventMessage"/>.</param>
		public override void Dispatch(INetworkMessageReceiver receiver, IMessageParameters parameters)
		{
			Throw<ArgumentNullException>.If.IsNull(receiver, nameof(receiver), $"{nameof(INetworkMessageReceiver)} parameter is null in {this.GetType().Name}");
			Throw<ArgumentNullException>.If.IsNull(parameters, nameof(parameters), $"{nameof(IMessageParameters)} parameter is null in {this.GetType().Name}");

			receiver.OnNetworkMessageReceive(this, parameters);
		}

		public override NetworkMessage DeepClone()
		{
			return new EventMessage(Payload.ShallowClone());
		}
	}
}
