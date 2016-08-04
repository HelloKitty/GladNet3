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
	/// <typeparam name="TPayloadType">Type of the payload</typeparam>
	public interface IRequestPayloadHandlerService<in TPeerType, TPayloadType> : IRequestPayloadHandlerService<TPeerType>, IRequestPayloadHandler<TPeerType, TPayloadType>
		where TPeerType : INetPeer where TPayloadType : PacketPayload
	{

	}

	/// <summary>
	/// IoC/Meta-data Marker for request handlers services.
	/// </summary>
	/// <typeparam name="TPeerType">Type of the peer.</typeparam>
	/// <typeparam name="TPayloadType">Type of the payload</typeparam>
	public interface IRequestPayloadHandlerService<in TPeerType> : IRequestPayloadHandler<TPeerType>
		where TPeerType : INetPeer
	{

	}
}
