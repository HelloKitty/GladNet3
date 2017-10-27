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
	/// Contract for client that exposed <see cref="IPacketHeader"/> reading
	/// capabilities.
	/// </summary>
	public interface IPacketHeaderReadable
	{
		/// <summary>
		/// Attempts to read a <see cref="IPacketHeader"/> from
		/// the client.
		/// </summary>
		/// <param name="token">Token that can be used to cancel the operation.</param>
		/// <returns>A PSOBBPacketHeader.</returns>
		Task<IPacketHeader> ReadHeaderAsync(CancellationToken token);
	}
}
