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
	public interface IPacketPayloadWritable<in TPayloadBaseType>
		where TPayloadBaseType : class
	{
		/// <summary>
		/// Writes the provided <see cref="payload"/> asyncrounously.
		/// </summary>
		/// <param name="payload">The payload to write.</param>
		/// <param name="token"></param>
		/// <returns>An awaitable task that will complete when the payload is sent.</returns>
		Task WriteAsync(TPayloadBaseType payload, CancellationToken token = default(CancellationToken));
	}
}
