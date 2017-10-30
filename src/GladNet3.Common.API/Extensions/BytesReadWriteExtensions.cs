using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GladNet
{
	public static class BytesReadWriteExtensions
	{
		/// <summary>
		/// Reads <see cref="count"/> many bytes from the reader.
		/// </summary>
		/// <param name="readable"></param>
		/// <param name="count">How many bytes to read.</param>
		/// <returns>The read bytes.</returns>
		public static byte[] Read(this IBytesReadable readable, int count)
		{
			if(readable == null) throw new ArgumentNullException(nameof(readable));

			return readable.ReadAsync(count, 0).Result;
		}

		/// <summary>
		/// Reads asyncronously <see cref="count"/> many bytes from the reader.
		/// </summary>
		/// <param name="readable"></param>
		/// <param name="count">How many bytes to read.</param>
		/// <param name="timeoutInMilliseconds">How many milliseconds to wait before canceling the operation.</param>
		/// <returns>A future for the read bytes.</returns>
		public static async Task<byte[]> ReadAsync(this IBytesReadable readable, int count, int timeoutInMilliseconds)
		{
			if(readable == null) throw new ArgumentNullException(nameof(readable));

			byte[] bytes = new byte[count];

			int resultCount = await readable.ReadAsync(bytes, 0, count, timeoutInMilliseconds)
				.ConfigureAwait(false);

			if(resultCount != count)
				throw new InvalidOperationException($"Failed to read {count} many bytes form {nameof(IBytesReadable)}. Read {resultCount} many bytes.");

			return bytes;
		}

		/// <summary>
		/// Reads asyncronously <see cref="count"/> many bytes from the reader.
		/// </summary>
		/// <param name="buffer">The buffer to store the bytes into.</param>
		/// <param name="start">The start position in the buffer to start reading into.</param>
		/// <param name="count">How many bytes to read.</param>
		/// <param name="timeoutInMilliseconds">How many milliseconds to wait before canceling the operation.</param>
		/// <returns>A future for the read bytes.</returns>
		public static async Task<int> ReadAsync(this IBytesReadable readable, byte[] buffer, int start, int count, int timeoutInMilliseconds)
		{
			if(readable == null) throw new ArgumentNullException(nameof(readable));

			return await readable.ReadAsync(buffer, 0, count, new CancellationTokenSource(timeoutInMilliseconds).Token)
				.ConfigureAwait(false);
		}

		/// <summary>
		/// Writes the provided <see cref="bytes"/>.
		/// </summary>
		/// <param name="writable"></param>
		/// <param name="bytes">The bytes to write.</param>
		public static void Write(this IBytesWrittable writable, byte[] bytes)
		{
			if(writable == null) throw new ArgumentNullException(nameof(writable));

			writable.Write(bytes, 0, bytes.Length);
		}

		/// <summary>
		/// Writes the provided <see cref="bytes"/> starting at the <see cref="offset"/>
		/// for <see cref="count"/> many bytes.
		/// </summary>
		/// <param name="writable"></param>
		/// <param name="bytes">The bytes to write.</param>
		/// <param name="offset">The offset to start at.</param>
		/// <param name="count">The amount of bytes to write.</param>
		public static void Write(this IBytesWrittable writable, byte[] bytes, int offset, int count)
		{
			if(writable == null) throw new ArgumentNullException(nameof(writable));

			writable.WriteAsync(bytes, offset, count);
		}

		/// <summary>
		/// Writes the provided <see cref="bytes"/> asyncronously.
		/// </summary>
		/// <param name="writable"></param>
		/// <param name="bytes">The bytes to write.</param>
		/// <returns>An awaitable task.</returns>
		public static async Task WriteAsync(this IBytesWrittable writable, byte[] bytes)
		{
			if(writable == null) throw new ArgumentNullException(nameof(writable));

			await writable.WriteAsync(bytes, 0, bytes.Length)
				.ConfigureAwait(false);
		}
	}
}
