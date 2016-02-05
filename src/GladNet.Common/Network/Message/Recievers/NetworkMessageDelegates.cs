using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	/// <summary>
	/// Delegate for <see cref="IEventMessage"/>s.
	/// </summary>
	/// <param name="eventMessage">Network message instance.</param>
	/// <param name="parameters">Message parameters for the network message.</param>
	public delegate void OnNetworkEventMessage(IEventMessage eventMessage, IMessageParameters parameters);

	/// <summary>
	/// Delegate for <see cref="IRequestMessage"/>s.
	/// </summary>
	/// <param name="requestMessage">Network message instance.</param>
	/// <param name="parameters">Message parameters for the network message.</param>
	public delegate void OnNetworkRequestMessage(IRequestMessage requestMessage, IMessageParameters parameters);

	/// <summary>
	/// Delegate for <see cref="IResponseMessage"/>s.
	/// </summary>
	/// <param name="responseMessage">Network message instance.</param>
	/// <param name="parameters">Message parameters for the network message.</param>
	public delegate void OnNetworkResponseMessage(IResponseMessage responseMessage, IMessageParameters parameters);

	/// <summary>
	/// Delegate for <see cref="IStatusMessage"/>s.
	/// </summary>
	/// <param name="statusMessage">Network message instance.</param>
	/// <param name="parameters">Message parameters for the network message.</param>
	public delegate void OnNetworkStatusMessage(IStatusMessage statusMessage, IMessageParameters parameters);
}
