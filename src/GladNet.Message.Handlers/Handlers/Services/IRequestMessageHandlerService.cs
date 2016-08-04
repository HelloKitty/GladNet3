using GladNet.Engine.Common;
using GladNet.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Message.Handlers
{
	/// <summary>
	/// IoC/Meta-data Marker for request handlers services.
	/// </summary>
	/// <typeparam name="TPeerType">Type of the peer.</typeparam>
	public interface IRequestMessageHandlerService<in TPeerType> : IRequestMessageHandler<TPeerType>
		where TPeerType : INetPeer
	{

	}
}
