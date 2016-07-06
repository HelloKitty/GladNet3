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
	/// Contract that guarantees implementing types offer some <see cref="INetworkMessage"/> and <see cref="PacketPayload"/> sending functionality.
	/// </summary>
	public interface INetworkMessageRouterService : INetSender, INetworkMessagePayloadSenderService
	{
		/// <summary>
		/// Tries to send the <typeparamref name="TMessageType"/> message without routing semantics.
		/// </summary>
		/// <typeparam name="TMessageType">A <see cref="INetworkMessage"/> type that implements <see cref="IRoutableMessage"/>.</typeparam>
		/// <param name="message"><typeparamref name="TMessageType"/> to be sent.</param>
		/// <param name="deliveryMethod">The deseried <see cref="DeliveryMethod"/> of the message.</param>
		/// <param name="encrypt">Indicates if the message should be encrypted.</param>
		/// <param name="channel">Indicates the channel for this message to be sent over.</param>
		/// <exception cref="InvalidOperationException">Throws this if the <see cref="IOperationTypeMappable"/> cannot map to a handable <see cref="OperationType"/>.</exception>
		/// <returns>Indication of the message send state.</returns>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		SendResult TryRouteMessage<TMessageType>(TMessageType message, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
			where TMessageType : INetworkMessage, IRoutableMessage, IOperationTypeMappable; //we need IOperationTypeMappable so that we don't have to abuse type introspection which is sorta a smell
	}
}
