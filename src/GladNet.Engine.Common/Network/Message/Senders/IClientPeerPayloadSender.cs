using GladNet.Common;
using GladNet.Message;
using GladNet.Payload;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace GladNet.Engine.Common
{
	/// <summary>
	/// Contracts implementing types to offer specific/supported <see cref="PacketPayload"/> sending such as; Request
	/// </summary>
	public interface IClientPeerPayloadSender
	{
		/// <summary>
		/// Sends a networked request.
		/// </summary>
		/// <param name="payload"><see cref="PacketPayload"/> for the desired network request message.</param>
		/// <param name="deliveryMethod">Desired <see cref="DeliveryMethod"/> for the request. See documentation for more information.</param>
		/// <param name="encrypt">Optional: Indicates if the message should be encrypted. Default: false</param>
		/// <param name="channel">Optional: Inidicates the channel the network message should be sent on. Default: 0</param>
		/// <returns>Indication of the message send state.</returns>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		SendResult SendRequest(PacketPayload payload, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0);

		/// <summary>
		/// Sends a networked request.
		/// Additionally this message/payloadtype is known to have static send parameters and those will be used in transit.
		/// </summary>
		/// <typeparam name="TPacketType">Type of the packet payload.</typeparam>
		/// <param name="payload">Payload instance to be sent in the message that contains static message parameters.</param>
		/// <returns>Indication of the message send state.</returns>
		SendResult SendRequest<TPacketType>(TPacketType payload)
			where TPacketType : PacketPayload, IStaticPayloadParameters;
	}
}
