using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GladNet;

namespace GladNet
{
	/// <summary>
	/// Contract for client that exposed <see cref="IPacketHeader"/> writing
	/// capabilities.
	/// </summary>
	public interface IPacketHeaderWritable
	{
		/// <summary>
		/// Attempts to write a <see cref="IPacketHeader"/> to the
		/// client.
		/// </summary>
		/// <param name="header">The packet header to write.</param>
		/// <param name="token"></param>
		/// <returns>An awaitable future that indicates the written/serialized size of the provided <see cref="header"/>.</returns>
		Task<int> WriteHeaderAsync(IPacketHeader header, CancellationToken token = default(CancellationToken));
	}
}
