using GladNet.Engine.Common;
using GladNet.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Message.Handlers
{
	/// <summary>
	/// Contract for <see cref="IResponseMessage"/> message handler.
	/// </summary>
	/// <typeparam name="TPeerType">Type of the peer.</typeparam>
	[MessageHandlerType(Common.OperationType.Response)]
	public interface IResponseMessageHandler<in TPeerType> : IMessageHandler<TPeerType, IResponseMessage>
		where TPeerType : INetPeer
	{

	}
}
