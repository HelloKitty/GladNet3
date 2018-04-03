using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GladNet
{
	/// <summary>
	/// Contract for strategies for dispatching network messages.
	/// </summary>
	public interface INetworkMessageDispatchingStrategy<TPayloadWriteType, TPayloadReadType> 
		where TPayloadWriteType : class 
		where TPayloadReadType : class
	{
		/// <summary>
		/// Message handling method.
		/// The task should complete not when the message is done being handled but when the
		/// session should continue reading messages. This means that if desired you should be able to read more
		/// messages even while other session's messages are being handled.
		/// </summary>
		/// <param name="context">The context for the incoming message.</param>
		/// <returns>A task that completes when the session should begin reading more messages.</returns>
		Task DispatchNetworkMessage(SessionMessageContext<TPayloadWriteType, TPayloadReadType> context);
	}
}
