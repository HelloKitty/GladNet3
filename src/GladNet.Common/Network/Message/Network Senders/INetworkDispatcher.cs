using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	/// <summary>
	/// Contract for types that offers the service of dispatching <see cref="NetworkMessage"/>s and <see cref="DispatchMessage"/>.
	/// </summary>
	public interface INetworkDispatcher
	{
		/// <summary>
		/// Attempts to enqueue a <see cref="DisaptchMessage"/> for servicing to the the network.
		/// </summary>
		/// <param name="message">The <see cref="DispatchMessage"/> to be processed and pushed to the network.</param>
		/// <returns>Indicates if enqueueing successful.</returns>
		bool TryEnqueue(DispatchMessage message);

		/// <summary>
		/// Attempts to enqueue a <see cref="NetworkMessage"/> for servicing to the the network providing <see cref="IMessageParameters"/> details
		/// for dispatching.
		/// </summary>
		/// <param name="message">The <see cref="NetworkMessage"/> to be processed and pushed to the network.</param>
		/// <param name="parameters">Non-optional <see cref="IMessageParameters"/> needed to process the message.</param>
		/// <returns>Indicates if enqueueing successful.</returns>
		bool TryEnqueue(NetworkMessage message, IMessageParameters parameters);
	}
}
