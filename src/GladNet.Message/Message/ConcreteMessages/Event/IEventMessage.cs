using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GladNet.Message
{
	/// <summary>
	/// <see cref="IEventMessage"/>s are <see cref="INetworkMessages"/>s that are unexpected responses to network events.
	/// Generally these are messages that are the result of other-client/server state changes that require 'unexpected' network messages
	/// to sync.
	/// </summary>
	public interface IEventMessage : INetworkMessage
	{

	}
}
