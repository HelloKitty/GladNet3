using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		/// <param name="mParams">The message parameters the message was sent with.</param>
		void OnNetworkMessageRecieve(IRequestMessage message, IMessageParameters mParams);


		/// <summary>
		/// Interface method overload for receiving a <see cref="IResponseMessage"/>.
		/// </summary>
		/// <param name="message">The response recieved from the remote peer.</param>
		/// <param name="mParams">The message parameters the message was sent with.</param>
		void OnNetworkMessageRecieve(IResponseMessage message, IMessageParameters mParams);


		/// <summary>
		/// Interface method overload for receiving a <see cref="IEventMessage"/>.
		/// </summary>
		/// <param name="message">The event recieved from the remote peer.</param>
		/// <param name="mParams">The message parameters the message was sent with.</param>
		void OnNetworkMessageRecieve(IEventMessage message, IMessageParameters mParams);
	}
}
