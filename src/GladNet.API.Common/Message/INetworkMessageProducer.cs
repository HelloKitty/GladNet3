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
	/// Contract for types that can produce network messages.
	/// </summary>
	/// <typeparam name="TPayloadBaseType">The network message type.</typeparam>
	public interface INetworkMessageProducer<TPayloadBaseType>
		where TPayloadBaseType : class
	{
		/// <summary>
		/// Produces a <see cref="NetworkIncomingMessage{TPayloadType}"/> asyncronously.
		/// The task will complete when a network message is available or when the token is cancelled.
		/// </summary>
		/// <returns>Returns a future that will complete when a message is available.</returns>
		Task<NetworkIncomingMessage<TPayloadBaseType>> ReadMessageAsync(CancellationToken token);

		//TODO: Do we need a syncronous version?
	}
}
