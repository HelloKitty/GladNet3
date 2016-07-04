using GladNet.Payload;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;


namespace GladNet.Common
{
	/// <summary>
	/// Contract that guarantees implementing types offer some <see cref="INetworkMessage"/> and <see cref="PacketPayload"/> sending functionality.
	/// </summary>
	public interface INetworkMessageRouterService : INetSender, INetworkMessagePayloadSenderService
	{
		/// <summary>
		/// Tries to send the <see cref="IResponseMessage"/> message without routing semantics.
		/// </summary>
		/// <param name="message"><see cref="IResponseMessage"/> to be sent.</param>
		/// <param name="deliveryMethod">The deseried <see cref="DeliveryMethod"/> of the message.</param>
		/// <param name="encrypt">Indicates if the message should be encrypted.</param>
		/// <param name="channel">Indicates the channel for this message to be sent over.</param>
		/// <returns>Indication of the message send state.</returns>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		SendResult TryRouteMessage(IResponseMessage message, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0);

		/// <summary>
		/// Tries to send the <see cref="IResponseMessage"/> message without routing semantics.
		/// </summary>
		/// <param name="message"><see cref="IResponseMessage"/> to be sent.</param>
		/// <param name="deliveryMethod">The deseried <see cref="DeliveryMethod"/> of the message.</param>
		/// <param name="encrypt">Indicates if the message should be encrypted.</param>
		/// <param name="channel">Indicates the channel for this message to be sent over.</param>
		/// <returns>Indication of the message send state.</returns>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		SendResult TryRouteMessage(IRequestMessage message, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0);
	}
}
