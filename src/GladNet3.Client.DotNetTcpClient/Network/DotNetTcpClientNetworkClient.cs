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

		/// <summary>
		/// Creates a new <see cref="DotNetTcpClientNetworkClient"/> with an intialized
		/// internal <see cref="InternalTcpClient"/>. If you want to supply your own
		/// <see cref="TcpClient"/> then call the ctor overload.
		/// </summary>
		public DotNetTcpClientNetworkClient()
		{
			InternalTcpClient = new TcpClient();
		}

		/// <summary>
		/// Creates a new <see cref="DotNetTcpClientNetworkClient"/> with the provided
		/// non-null <see cref="tcpClient"/>. This overload should be used if you for some reason
		/// want to use an externally created <see cref="TcpClient"/>.
		/// </summary>
		/// <param name="tcpClient">The <see cref="TcpClient"/> to use.</param>
		public DotNetTcpClientNetworkClient(TcpClient tcpClient)
		{
			if(tcpClient == null) throw new ArgumentNullException(nameof(tcpClient));

			InternalTcpClient = tcpClient;
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
		public override Task ClearReadBuffers()
		{
			//We can't do anything, there is no buffer
			return Task.CompletedTask;
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
		public override Task WriteAsync(byte[] bytes, int offset, int count)
		{
			if(!InternalTcpClient.Connected)
				throw new InvalidOperationException($"The internal {nameof(TcpClient)}: {nameof(InternalTcpClient)} is not connected to an endpoint. You must call {nameof(ConnectAsync)} before writing any bytes.");

			//We can just write the bytes to the stream if we're connected.
			return InternalTcpClient.GetStream().WriteAsync(bytes, offset, count);
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
				{
					int countRead = await stream.ReadAsync(buffer, i, end - i, token)
						.ConfigureAwait(false);

					if(countRead == 0)
						throw new InvalidOperationException($"Encounted possible one-way shutdown from network. Read 0 bytes.");

					i += countRead;
				}
					
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
