using GladNet.Engine.Common;
using GladNet.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Message.Handlers
{
	/// <summary>
	/// Contract for <see cref="IRequestMessage"/> message handler.
	/// </summary>
	/// <typeparam name="TPeerType">Type of the peer.</typeparam>
	[MessageHandlerType(Common.OperationType.Request)]
	public interface IRequestMessageHandler<in TPeerType> : IMessageHandler<TPeerType, IRequestMessage>
		where TPeerType : INetPeer
	{

	}
}
