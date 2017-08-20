using GladNet.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Lidgren.Engine.Common
{
	/// <summary>
	/// Contract for strategy that produces <see cref="INetworkMessageRouterService"/>s based on
	/// the provided connectionId.
	/// </summary>
	public interface ISendServiceSelectionStrategy
	{
		INetworkMessagePayloadSenderService GetSendingService(int connectionId);
	}
}
