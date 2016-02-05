using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;


namespace GladNet.Common
{
	/// <summary>
	/// Contract that guarantees implementing types offer some network message sending functionality.
	/// </summary>
	public interface INetworkMessageSender : INetSender
	{
		/// <summary>
		/// Attempts to send a message; may fail and failure will be reported.
		/// </summary>
		/// <param name="opType"><see cref="OperationType"/> of the message to send.</param>
		/// <param name="payload">Payload instance to be sent in the message.</param>
		/// <param name="deliveryMethod">The deseried <see cref="DeliveryMethod"/> of the message.</param>
		/// <param name="encrypt">Indicates if the message should be encrypted.</param>
		/// <param name="channel">Indicates the channel for this message to be sent over.</param>
		/// <returns>Indication of the message send state.</returns>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		SendResult TrySendMessage(OperationType opType, PacketPayload payload, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0);

		/// <summary>
		/// Attempts to send a message; may fail and failure will be reported.
		/// Additionally this message/payloadtype is known to have static send parameters and those will be used in transit.
		/// </summary>
		/// <typeparam name="TPacketType">Type of the packet payload.</typeparam>
		/// <param name="opType"><see cref="OperationType"/> of the message to send.</param>
		/// <param name="payload">Payload instance to be sent in the message that contains static message parameters.</param>
		/// <returns>Indication of the message send state.</returns>
		SendResult TrySendMessage<TPacketType>(OperationType opType, TPacketType payload)
			where TPacketType : PacketPayload, IStaticPayloadParameters;
	}
}
