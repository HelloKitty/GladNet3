using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	/// <summary>
	/// Begins a subscription request
	/// (Fluent builder that carries data)
	/// </summary>
	/// <typeparam name="TNetworkMessageType">Type of <see cref="NetworkMessage"/> to subscribe to.</typeparam>
	public interface INetworkMessageSubcriptionFluentBuilder<TNetworkMessageType>
		where TNetworkMessageType : INetworkMessage
	{
		/// <summary>
		/// Subscription service to carry for fluent.
		/// </summary>
		INetworkMessageSubscriptionService Service { get; }
	}
}
