using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladLive.Common
{
	/// <summary>
	/// IoC/Meta-data Marker for Event handlers.
	/// </summary>
	/// <typeparam name="TPeerType">Type of the peer.</typeparam>
	/// <typeparam name="TPayloadType">Type of the payload</typeparam>
	public interface IEventPayloadHandler<in TPeerType, TPayloadType> : IEventPayloadHandler<TPeerType>, IPayloadHandler<TPeerType, TPayloadType>
		where TPeerType : INetPeer where TPayloadType : PacketPayload
	{

	}

	/// <summary>
	/// IoC/Meta-data Marker for Event handlers.
	/// </summary>
	/// <typeparam name="TPeerType">Type of the peer.</typeparam>
	/// <typeparam name="TPayloadType">Type of the payload</typeparam>
	public interface IEventPayloadHandler<in TPeerType> : IPayloadHandler<TPeerType>
		where TPeerType : INetPeer
	{

	}
}
