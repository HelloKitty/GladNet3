using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Pipelines.Sockets.Unofficial;

namespace GladNet
{
	/// <summary>
	/// Implementation of <see cref="IConnectionService"/> based around <see cref="SocketConnection"/>
	/// </summary>
	public sealed class SocketConnectionConnectionServiceAdapter : IConnectionService
	{
		private SocketConnection Connection { get; }

		public bool isConnected => Connection.Socket.Connected;

		public SocketConnectionConnectionServiceAdapter(SocketConnection connection)
		{
			Connection = connection ?? throw new ArgumentNullException(nameof(connection));
		}

		public Task DisconnectAsync()
		{
			//TODO: We don't have a mechanism to flush??
			Connection.Dispose();
			return Task.CompletedTask;
		}

		//TODO: This is kind of stupid to even implement. This should NEVER be called on serverside!
		/// <inheritdoc />
		public async Task<bool> ConnectAsync(string ip, int port)
		{
			if (Connection.Socket.Connected)
				return false;

			await Connection.Socket.ConnectAsync(ip, port);
			return Connection.Socket.Connected;
		}
	}
}
