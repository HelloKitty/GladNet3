using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GladNet
{
	/// <summary>
	/// DotNet <see cref="WebSocket"/> implementation of the <see cref="IWebSocketConnection"/>.
	/// </summary>
	public sealed class DotNetWebSocketConnection : IWebSocketConnection
	{
		/// <summary>
		/// The adapted dotnet socket.
		/// </summary>
		private WebSocket Connection { get; }

		/// <inheritdoc />
		public WebSocketState State => Connection.State;

		/// <inheritdoc />
		public WebSocketCloseStatus? CloseStatus => Connection.CloseStatus;

		/// <summary>
		/// Creates a new <see cref="DotNetWebSocketConnection"/> that implements <see cref="IWebSocketConnection"/>
		/// adapting the provided <see cref="WebSocket"/> connection.
		/// </summary>
		/// <param name="connection"></param>
		public DotNetWebSocketConnection(WebSocket connection)
		{
			Connection = connection ?? throw new ArgumentNullException(nameof(connection));
		}

		/// <inheritdoc />
		public Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken = default)
		{
			return Connection.CloseAsync(closeStatus, statusDescription, cancellationToken);
		}

		/// <inheritdoc />
		public Task ConnectAsync(Uri uri, CancellationToken cancellationToken = default)
		{
			if (Connection is ClientWebSocket cws)
			{
				return cws.ConnectAsync(uri, CancellationToken.None);
			}
			else
				throw new NotSupportedException($"It is not supported to call {nameof(ConnectAsync)} on a non-client websocket.");
		}

		public async Task ReceiveAsync(byte[] buffer, int count, CancellationToken token = default)
		{
			ArraySegment<byte> bufferSegment = new ArraySegment<byte>(buffer, 0, count);

			do
			{
				WebSocketReceiveResult result
					= await Connection.ReceiveAsync(bufferSegment, token);

				var totalBytesRead = bufferSegment.Offset + result.Count;

				// Read the buffer, don't rely on it being EndOfMessage. We might have the payload as apart of the same message
				if(totalBytesRead
				   == count)
					break;
				else if (totalBytesRead > count)
					throw new InvalidOperationException($"Read more bytes than request. Read: {totalBytesRead} Expected: {count}.");

				// Move the segment forward
				bufferSegment = new ArraySegment<byte>(buffer, bufferSegment.Offset + result.Count, bufferSegment.Count - result.Count);

			} while(!token.IsCancellationRequested
			        && Connection.State == WebSocketState.Open);
		}

		/// <inheritdoc />
		public async Task SendAsync(ArraySegment<byte> buffer, bool endMessage, CancellationToken token = default)
		{
			await Connection.SendAsync(buffer, WebSocketMessageType.Binary, endMessage, token);
		}

		public void Dispose()
		{
			Connection.Dispose();
		}
	}
}
