using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GladNet
{
	/// <summary>
	/// Contract for types that can write a packet payload.
	/// </summary>
	public interface IPacketPayloadReadable<TPayloadBaseType>
		where TPayloadBaseType : class
	{
		/// <summary>
		/// Reads an incoming message asyncronously.
		/// The task will complete when an incomding message can be built.
		/// </summary>
		/// <param name="token">The token that can cancel the operation.</param>
		/// <returns>A future for the resulting incoming message.</returns>
		Task<NetworkIncomingMessage<TPayloadBaseType>> ReadAsync(CancellationToken token);
	}
}
