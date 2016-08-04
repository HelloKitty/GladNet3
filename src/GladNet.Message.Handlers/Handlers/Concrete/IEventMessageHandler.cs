using GladNet.Engine.Common;
using GladNet.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Message.Handlers
{
	/// <summary>
	/// Contract for <see cref="IEventMessage"/> message handler.
	/// </summary>
	/// <typeparam name="TPeerType">Type of the peer.</typeparam>
	[MessageHandlerType(Common.OperationType.Event)]
	public interface IEventMessageHandler<in TPeerType> : IMessageHandler<TPeerType, IEventMessage>
		where TPeerType : INetPeer
	{

	}
}
