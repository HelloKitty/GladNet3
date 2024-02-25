using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GladNet
{
	public interface IWebSocketConnection
	{
		/// <summary>
		/// Closes the WebSocket connection as an asynchronous operation using the close handshake defined in the WebSocket protocol specification section 7.
		/// </summary>
		/// <param name="closeStatus">Indicates the reason for closing the WebSocket connection.</param>
		/// <param name="statusDescription">Specifies a human readable explanation as to why the connection is closed.</param>
		/// <param name="cancellationToken">The token that can be used to propagate notification that operations should be canceled.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken = default);

		/// <summary>
		/// Returns the current state of the WebSocket connection.
		/// </summary>
		WebSocketState State { get; }

		/// <summary>
		/// Indicates the reason why the remote endpoint initiated the close handshake.
		/// If the close handshake has not been initiated yet, WebSocketCloseStatus.None is returned.
		/// </summary>
		WebSocketCloseStatus? CloseStatus { get; }
	}
}
