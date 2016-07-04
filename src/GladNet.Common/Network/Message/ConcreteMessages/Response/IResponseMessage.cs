using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GladNet.Common
{
	/// <summary>
	/// <see cref="IResponseMessage"/>s are <see cref="INetworkMessage"/>s in response to <see cref="IRequestMessage"/> from remote peers.
	/// It contains additional fields/properties compared to <see cref="INetworkMessage"/> that provide information on the response.
	/// </summary>
	public interface IResponseMessage : INetworkMessage
#if !ENDUSER
		, IRoutableMessage
#endif
	{

	}
}
