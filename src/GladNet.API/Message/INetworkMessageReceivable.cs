using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GladNet
{
	/// <summary>
	/// Contract for types that handle a received network message.
	/// </summary>
	/// <typeparam name="TPayloadType"></typeparam>
	public interface INetworkMessageReceivable<TPayloadType>
		where TPayloadType : class
	{
		/// <summary>
		/// Called when a network message is recieved.
		/// Can also be called to simulate the recieveing a network message.
		/// </summary>
		/// <param name="message">The network message recieved.</param>
		/// <returns></returns>
		Task OnNetworkMessageReceived(NetworkIncomingMessage<TPayloadType> message);
	}
}