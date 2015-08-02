using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	public interface IResponsePayload
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
