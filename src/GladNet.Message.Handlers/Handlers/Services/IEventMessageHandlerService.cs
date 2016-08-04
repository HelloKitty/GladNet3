using GladNet.Engine.Common;
using GladNet.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Message.Handlers
{
	/// <summary>
	/// IoC/Meta-data Marker for Event handlers services.
	/// </summary>
	/// <typeparam name="TPeerType">Type of the peer.</typeparam>
	public interface IEventMessageHandlerService<in TPeerType> : IEventMessageHandler<TPeerType>
		where TPeerType : INetPeer
	{

	}
}
