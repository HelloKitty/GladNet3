using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GladNet
{
	/// <summary>
	/// The default in place dispatching strategy for network messages which
	/// handles them immediately without any enqueueing and will yield back when
	/// the message has been completed.
	/// </summary>
	/// <typeparam name="TPayloadWriteType"></typeparam>
	/// <typeparam name="TPayloadReadType"></typeparam>
	public sealed class InPlaceNetworkMessageDispatchingStrategy<TPayloadWriteType, TPayloadReadType> : INetworkMessageDispatchingStrategy<TPayloadWriteType, TPayloadReadType>
		where TPayloadWriteType : class
		where TPayloadReadType : class
	{
		/// <inheritdoc />
		public Task DispatchNetworkMessage(SessionMessageContext<TPayloadWriteType, TPayloadReadType> context)
		{
			//The default implementation (or in place implementation) dispatches the message asyncronously
			//in the current context without any enqueueing or waiting.
			return context.Session.OnNetworkMessageRecieved(context.Message);
		}
	}
}
