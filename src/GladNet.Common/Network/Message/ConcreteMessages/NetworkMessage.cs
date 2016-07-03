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
	public abstract class NetworkMessage : INetworkMessage, IDeepCloneable<NetworkMessage>
	{
		/// <summary>
		/// The payload of a <see cref="INetworkMessage"/>. Can be sent accross a network.
		/// <see cref="NetSendable"/> enforces its wire readyness.
		/// </summary>
		[GladNetMember(GladNetPayloadDataIndex.Index1, IsRequired = true)]
		public NetSendable<PacketPayload> Payload { get; private set; }

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
			if (sendPayload == null)
				throw new ArgumentNullException("netSendablePacket", "A null Netsendable<PacketPayload> was passed for NetworkMessage creation."); //ncrunch: no coverage

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
			return this.DeepClone();
		}
	}
}
