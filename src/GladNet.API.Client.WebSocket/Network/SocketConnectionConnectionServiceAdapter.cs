using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GladNet
{
	/// <summary>
	/// Implementation of <see cref="INetworkConnectionService"/> based around <see cref="WebSocket"/>
	/// </summary>
	public sealed class SocketConnectionConnectionServiceAdapter : INetworkConnectionService
	{
		/// <summary>
		/// Internal socket connection.
		/// </summary>
		private WebSocket Connection { get; }

		/// <inheritdoc />
		public bool isConnected => (Connection.State == WebSocketState.Open || Connection.State == WebSocketState.Connecting)
		                           && !Connection.CloseStatus.HasValue;

		public SocketConnectionConnectionServiceAdapter(WebSocket connection)
		{
			Connection = connection ?? throw new ArgumentNullException(nameof(connection));
		}

		/// <inheritdoc />
		public async Task DisconnectAsync()
		{
			await Connection.CloseAsync(WebSocketCloseStatus.NormalClosure, String.Empty, CancellationToken.None);
			Connection.Dispose();
		}

		//TODO: This is kind of stupid to even implement. This should NEVER be called on serverside!
		/// <inheritdoc />
		public async Task<bool> ConnectAsync(string ip, int port)
		{
			if (isConnected)
				return false;

			if (Connection is ClientWebSocket cws)
			{
				await cws.ConnectAsync(new Uri(ip), CancellationToken.None);
				return Connection.State == WebSocketState.Open;
			}
			else
				throw new NotSupportedException($"It is not supported to call {nameof(ConnectAsync)} on a non-client websocket.");
		}
	}
}
