using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GladNet.Common
{
	/// <summary>
	/// A contract for receivers of <see cref="NetworkMessage"/> interface subtypes.
	/// </summary>
	public interface INetworkMessageReceiver
	{
		/// <summary>
		/// Interface method overload for receiving a <see cref="IRequestMessage"/>.
		/// </summary>
		/// <param name="message">The request recieved from the remote peer.</param>
		/// <param name="parameters">The message parameters the message was sent with.</param>
		void OnNetworkMessageReceive(IRequestMessage message, IMessageParameters parameters);

		/// <summary>
		/// Interface method overload for receiving a <see cref="IResponseMessage"/>.
		/// </summary>
		/// <param name="message">The response recieved from the remote peer.</param>
		/// <param name="parameters">The message parameters the message was sent with.</param>
		void OnNetworkMessageReceive(IResponseMessage message, IMessageParameters parameters);


		/// <summary>
		/// Interface method overload for receiving a <see cref="IEventMessage"/>.
		/// </summary>
		/// <param name="message">The event recieved from the remote peer.</param>
		/// <param name="parameters">The message parameters the message was sent with.</param>
		void OnNetworkMessageReceive(IEventMessage message, IMessageParameters parameters);

		/// <summary>
		/// Dispatchable method that handles <see cref="IStatusMessage"/> changes.
		/// </summary>
		/// <param name="status">The status message recieved from the remote peer.</param>
		void OnNetworkMessageReceive(IStatusMessage status, IMessageParameters parameters);
	}
}
