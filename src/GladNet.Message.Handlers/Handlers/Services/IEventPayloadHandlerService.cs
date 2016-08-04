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
	/// <typeparam name="TPayloadType">Type of the payload</typeparam>
	public interface IEventPayloadHandlerService<in TPeerType, TPayloadType> : IEventPayloadHandlerService<TPeerType>, IEventPayloadHandler<TPeerType, TPayloadType>
		where TPeerType : INetPeer where TPayloadType : PacketPayload
	{

	}

	/// <summary>
	/// IoC/Meta-data Marker for Event handlers services.
	/// </summary>
	/// <typeparam name="TPeerType">Type of the peer.</typeparam>
	/// <typeparam name="TPayloadType">Type of the payload</typeparam>
	public interface IEventPayloadHandlerService<in TPeerType> : IEventPayloadHandler<TPeerType>
		where TPeerType : INetPeer
	{

	}
}
