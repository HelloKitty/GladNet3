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
	/// Contracts implementing types to offer specific/supported network message types such as; Request
	/// </summary>
	public interface IClientPeerNetworkMessageRouter
	{
		/// <summary>
		/// Routes a <see cref="IRequestMessage"/> object.
		/// </summary>
		/// <param name="message"><see cref="IRequestMessage"/> desired network request message to send.</param>
		/// <param name="deliveryMethod">Desired <see cref="DeliveryMethod"/> for the request. See documentation for more information.</param>
		/// <param name="encrypt">Optional: Indicates if the message should be encrypted. Default: false</param>
		/// <param name="channel">Optional: Inidicates the channel the network message should be sent on. Default: 0</param>
		/// <returns>Indication of the message send state.</returns>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		SendResult RouteRequest(IRequestMessage message, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0);
	}
}
