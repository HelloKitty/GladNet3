using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	/// <summary>
	/// <see cref="IResponseMessage"/>s are <see cref="INetworkMessage"/>s in response to <see cref="IRequestMessage"/> from remote peers.
	/// It contains additional fields/properties compared to <see cref="INetworkMessage"/> that provide information on the response.
	/// </summary>
	public interface IResponseMessage : INetworkMessage
	{
		/// <summary>
		/// Indicates the response state of the <see cref="INetworkMessage"/>.
		/// 0 - Generally Failure
		/// 1 - Generally Sucess
		/// 2+ - User-Defined
		/// </summary>
		byte ResponseCode { get; }
	}
}
