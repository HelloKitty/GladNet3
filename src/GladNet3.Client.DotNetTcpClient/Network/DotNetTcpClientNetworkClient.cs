using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GladNet
{
	//TODO: Make this thread safe
	/// <summary>
	/// Base implementation of a TCP network client for PSOBB.
	/// It is built around the <see cref="TcpClient"/> provided in .NET and manages, destroys
	/// and creates them depending on the externally provided API.
	/// </summary>
	public sealed class DotNetTcpClientNetworkClient : NetworkClientBase, IConnectable, IDisconnectable, IDisposable,
		IBytesWrittable, IBytesReadable
	{
		//Can't be readonly because clients may want to reconnect
		private TcpClient InternalTcpClient { get; set; }

		public DotNetTcpClientNetworkClient()
		{
			InternalTcpClient = new TcpClient();
		}

		/// <inheritdoc />
		public override async Task<bool> ConnectAsync(string address, int port)
		{
			if(address == null) throw new ArgumentNullException(nameof(address));
			if(port <= 0) throw new ArgumentOutOfRangeException(nameof(port));

			await DisconnectAsync(10)
				.ConfigureAwait(false);

			InternalTcpClient = new TcpClient();

			//TODO: Logging
			//TODO: Should we allow reconnects?
			await InternalTcpClient.ConnectAsync(address, port)
				.ConfigureAwait(false);

			return true;
		}

		/// <inheritdoc />
		public override Task DisconnectAsync(int delay)
		{
			if(InternalTcpClient == null)
				return Task.CompletedTask;

			//TODO: Is this ok? Will it still work on netstandard?
#if NET46
			if(InternalTcpClient.Connected)
				InternalTcpClient.GetStream().Close(delay);

			InternalTcpClient.Close();
#endif
			InternalTcpClient.Dispose();

			return Task.CompletedTask;
		}

		/// <inheritdoc />
		public override async Task WriteAsync(byte[] bytes, int offset, int count)
		{
			if(!InternalTcpClient.Connected)
				throw new InvalidOperationException($"The internal {nameof(TcpClient)}: {nameof(InternalTcpClient)} is not connected to an endpoint. You must call {nameof(ConnectAsync)} before writing any bytes.");

			//We can just write the bytes to the stream if we're connected.
			await InternalTcpClient.GetStream().WriteAsync(bytes, offset, count)
				.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public override async Task<int> ReadAsync(byte[] buffer, int start, int count, CancellationToken token)
		{
			if(!InternalTcpClient.Connected)
				throw new InvalidOperationException($"The internal {nameof(TcpClient)}: {nameof(InternalTcpClient)} is not connected to an endpoint. You must call {nameof(ConnectAsync)} before reading any bytes.");

			NetworkStream stream = InternalTcpClient.GetStream();

			//Sockets nor NetworkStreams allow us to cancel
			//They will block even if you give them the token and then
			//throw when disposed or closed
			//So we do a check on the token in the catch to see if it threw
			//because of a requested cancellation
			int i = 0;
			try
			{
				int end = count + start;
				for(i = start; i < end && !token.IsCancellationRequested;)
					i += await stream.ReadAsync(buffer, i, end - i, token)
						.ConfigureAwait(false);
			}
			catch(Exception e)
			{
				if(token.IsCancellationRequested)
					return 0;

				//If it wasn't because of a cancelled token then we should throw
				throw new InvalidOperationException($"Failed to read from network. Offset: {start} Count: {count}", e);
			}

			return i - start;
		}

		private bool disposedValue = false; // To detect redundant calls

		protected override void Dispose(bool disposing)
		{
			if(!disposedValue)
			{
				if(disposing)
				{
#if NET46
					InternalTcpClient.GetStream().Close();
					InternalTcpClient.Close();
#endif
					InternalTcpClient.Dispose();
				}


				disposedValue = true;
			}
		}

		// ~PSOBBNetworkClient() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// GC.SuppressFinalize(this);
		}
	}
}
