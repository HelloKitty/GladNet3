using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;


namespace GladNet.Common
{
	/// <summary>
	/// Contract that guarantees implementing types provide general network message sending functionality.
	/// Also contracts implementing types to offer specific/supported network message types such as; Response and Event.
	/// </summary>
	public interface IClientNetworkMessageSender : INetworkMessageSender
	{
		/// <summary>
		/// Sends a networked response.
		/// </summary>
		/// <param name="payload"><see cref="PacketPayload"/> for the desired network response message.</param>
		/// <param name="deliveryMethod">Desired <see cref="NetworkMessage.DeliveryMethod"/> for the response. See documentation for more information.</param>
		/// <param name="encrypt">Optional: Indicates if the message should be encrypted. Default: false</param>
		/// <param name="channel">Optional: Inidicates the channel the network message should be sent on. Default: 0</param>
		/// <returns>Indicates the result of operation.</returns>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		NetworkMessage.SendResult SendResponse(PacketPayload payload, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0);

		/// <summary>
		/// Sends a networked event.
		/// </summary>
		/// <param name="payload"><see cref="PacketPayload"/> for the desired network event message.</param>
		/// <param name="deliveryMethod">Desired <see cref="NetworkMessage.DeliveryMethod"/> for the event. See documentation for more information.</param>
		/// <param name="encrypt">Optional: Indicates if the message should be encrypted. Default: false</param>
		/// <param name="channel">Optional: Inidicates the channel the network message should be sent on. Default: 0</param>
		/// <returns>Indicates the result of operation.</returns>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		NetworkMessage.SendResult SendEvent(PacketPayload payload, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0);
	}
}
