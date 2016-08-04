using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladLive.Common
{
	/// <summary>
	/// IoC/Meta-data Marker for Response handlers.
	/// </summary>
	/// <typeparam name="TPeerType">Type of the peer.</typeparam>
	/// <typeparam name="TPayloadType">Type of the payload</typeparam>
	public interface IResponsePayloadHandler<in TPeerType, TPayloadType> : IResponsePayloadHandler<TPeerType>, IPayloadHandler<TPeerType, TPayloadType>
		where TPeerType : INetPeer where TPayloadType : PacketPayload
	{

	}

	/// <summary>
	/// IoC/Meta-data Marker for Response handlers.
	/// </summary>
	/// <typeparam name="TPeerType">Type of the peer.</typeparam>
	/// <typeparam name="TPayloadType">Type of the payload</typeparam>
	public interface IResponsePayloadHandler<in TPeerType> : IPayloadHandler<TPeerType>
		where TPeerType : INetPeer
	{

	}
}
