using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GladNet
{
	public interface IBytesReadable
	{
		/// <summary>
		/// Reads asyncronously <see cref="count"/> many bytes from the reader.
		/// </summary>
		/// <param name="buffer">The buffer to store the bytes into.</param>
		/// <param name="start">The start position in the buffer to start reading into.</param>
		/// <param name="count">How many bytes to read.</param>
		/// <param name="token">The cancel token to check during the async operation.</param>
		/// <returns>A future for how many bytes were read.</returns>
		Task<int> ReadAsync(byte[] buffer, int start, int count, CancellationToken token);
	}
}
