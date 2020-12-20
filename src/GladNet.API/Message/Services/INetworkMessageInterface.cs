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
	/// Contract for a type that provided read/write interface for network messages
	/// and payloads.
	/// </summary>
	/// <typeparam name="TPayloadReadType">The incoming/read payload type.</typeparam>
	/// <typeparam name="TPayloadWriteType">The outgoing/written payload type.</typeparam>
	public interface INetworkMessageInterface<TPayloadReadType, in TPayloadWriteType>
		where TPayloadReadType : class
		where TPayloadWriteType : class
	{
		/// <summary>
		/// Produces a <see cref="NetworkIncomingMessage{TPayloadType}"/> asyncronously.
		/// The task will complete when a network message is available or when the token is cancelled.
		/// </summary>
		/// <returns>Returns a future that will complete when a message is available.</returns>
		Task<NetworkIncomingMessage<TPayloadReadType>> ReadMessageAsync(CancellationToken token = default(CancellationToken));

		//Implementer of this method should assume multi callers may call this method at one time
		//Therefore threadsafety must be implemented an assured by the implementer of this method.
		/// <summary>
		/// Sends a <typeparamref name="TPayloadWriteType"/> asyncronously.
		/// The task will complete when the network message is sent or when the token is cancelled.
		/// </summary>
		/// <param name="payload"></param>
		/// <param name="token"></param>
		/// <returns>Returns an awaitable when the send operation is completed.</returns>
		Task<SendResult> SendMessageAsync(TPayloadWriteType payload, CancellationToken token = default(CancellationToken));
	}
}
