using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Engine.Common
{
	/// <summary>
	/// Implementer offers the ability for consumers to subscribe to publishing channels.
	/// </summary>
	public interface INetworkMessagePublisher
	{
		/// <summary>
		/// Event channel.
		/// </summary>
		event OnNetworkEventMessage EventPublisher;

		/// <summary>
		/// Request channel.
		/// </summary>
		event OnNetworkRequestMessage RequestPublisher;

		/// <summary>
		/// Response channel.
		/// </summary>
		event OnNetworkResponseMessage ResponsePublisher;

		/// <summary>
		/// Status channel.
		/// </summary>
		event OnNetworkStatusMessage StatusPublisher;
	}
}
