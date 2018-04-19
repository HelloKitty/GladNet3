using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GladNet
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TPayloadType"></typeparam>
	public interface INetworkMessageReceivable<TPayloadType> 
		where TPayloadType : class
	{
		//TODO: We may also want to provide some message parameters when we add UDP support
		/// <summary>
		/// Called when a network message is recieved.
		/// Can also be called to simulate the recieveing a network message.
		/// </summary>
		/// <param name="message">The network message recieved.</param>
		/// <returns></returns>
		Task OnNetworkMessageRecieved(NetworkIncomingMessage<TPayloadType> message);
	}
}
