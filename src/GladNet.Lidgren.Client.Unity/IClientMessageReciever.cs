using GladNet.Common;
using GladNet.Message;
using GladNet.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Lidgren.Client.Unity
{
	//We only really expose this stuff publicaly for: simulation and unit testing purposes.
	/// <summary>
	/// Contract for payload and message recieveing semantics.
	/// </summary>
	public interface IClientNetworkMessageReciever
	{
		/// <summary>
		/// Called internally when a response is recieved.
		/// </summary>
		/// <param name="payload"><see cref="IResponseMessage"/> sent by the peer.</param>
		/// <param name="parameters">Parameters the message was sent with.</param>
		void OnReceiveResponse(IResponseMessage message, IMessageParameters parameters);

		/// <summary>
		/// Called internally when an event is recieved.
		/// </summary>
		/// <param name="message"><see cref="IEventMessage"/> sent by the peer.</param>
		/// <param name="parameters">Parameters the message was sent with.</param>
		void OnReceiveEvent(IEventMessage message, IMessageParameters parameters);

		/// <summary>
		/// Called internaly when a status changed is recieved.
		/// </summary>
		/// <param name="status">New <see cref="NetStatus"/> that has been dispatched.</param>
		void OnStatusChanged(NetStatus status);
	}
}