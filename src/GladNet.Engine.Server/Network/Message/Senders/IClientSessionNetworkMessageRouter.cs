using GladNet.Common;
using GladNet.Message;
using GladNet.Payload;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace GladNet.Engine.Server
{
	/// <summary>
	/// Contracts implementing types to offer specific/supported network message routing; Response and Event.
	/// </summary>
	public interface IClientSessionNetworkMessageRouter
	{
		/// <summary>
		/// Routes a <see cref="IResponseMessage"/> message.
		/// </summary>
		/// <param name="message"><see cref="IResponseMessage"/> to route.</param>
		/// <param name="deliveryMethod">Desired <see cref="DeliveryMethod"/> for the response. See documentation for more information.</param>
		/// <param name="encrypt">Optional: Indicates if the message should be encrypted. Default: false</param>
		/// <param name="channel">Optional: Inidicates the channel the network message should be sent on. Default: 0</param>
		/// <returns>Indication of the message send state.</returns>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		SendResult RouteResponse(IResponseMessage message, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0);
	}
}
