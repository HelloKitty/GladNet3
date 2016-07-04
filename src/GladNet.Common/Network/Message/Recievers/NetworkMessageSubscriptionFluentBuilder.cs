using Easyception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	/// <summary>
	/// Concrete fluent builder that carries data for fluent extensions
	/// </summary>
	/// <typeparam name="TNetworkMessageType"></typeparam>
	public class NetworkMessageSubscriptionFluentBuilder<TNetworkMessageType> : INetworkMessageSubcriptionFluentBuilder<TNetworkMessageType>
		where TNetworkMessageType : INetworkMessage
	{
		/// <summary>
		/// Subscription service to carry for fluent.
		/// </summary>
		public INetworkMessageSubscriptionService Service { get; private set; }

		public NetworkMessageSubscriptionFluentBuilder(INetworkMessageSubscriptionService service)
		{
			Throw<ArgumentNullException>.If.IsNull(service)
				?.Now(nameof(service));

			Service = service;
		}
	}
}
