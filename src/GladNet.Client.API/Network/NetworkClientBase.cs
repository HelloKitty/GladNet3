using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GladNet
{
	/// <summary>
	/// Base class for all network clients.
	/// </summary>
	public abstract class NetworkClientBase : IConnectable, IDisconnectable, IDisposable,
		IBytesWrittable, IBytesReadable
	{
		//Clients need only to implement the async subset methods
		//for the client to function. This will save a lot of duplication for potential
		//consumers of this base type

		/// <summary>
		/// Reads asyncronously <see cref="count"/> many bytes from the reader.
		/// </summary>
		/// <param name="buffer">The buffer to store the bytes into.</param>
		/// <param name="start">The start position in the buffer to start reading into.</param>
		/// <param name="count">How many bytes to read.</param>
		/// <param name="token">The cancel token to check during the async operation.</param>
		/// <returns>A future for how many bytes were read.</returns>
		public abstract Task<int> ReadAsync(byte[] buffer, int start, int count, CancellationToken token);

		/// <inheritdoc />
		public abstract Task DisconnectAsync(int delay);

		/// <inheritdoc />
		public abstract Task WriteAsync(byte[] bytes, int offset, int count);

		/// <inheritdoc />
		public abstract Task<bool> ConnectAsync(string ip, int port);

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if(!disposedValue)
			{
				if(disposing)
				{
				}


				disposedValue = true;
			}
		}

		// ~NetworkClientBase() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		void IDisposable.Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// GC.SuppressFinalize(this);
		}
		#endregion
	}
}
