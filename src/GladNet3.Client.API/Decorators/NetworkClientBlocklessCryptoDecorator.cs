using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace GladNet
{
	/// <summary>
	/// Crypto decroator for the <see cref="NetworkClientBase"/> that extends the <see cref="ReadAsync"/> and
	/// <see cref="WriteAsync"/> methods with an encryption implementation. It does not enforce any crypto blocksize.
	/// </summary>
	public sealed class NetworkClientBlocklessCryptoDecorator : NetworkClientBase, IConnectable, IDisconnectable, IDisposable,
		IBytesWrittable, IBytesReadable
	{
		/// <summary>
		/// The decorated <see cref="NetworkClientBase"/>.
		/// </summary>
		private NetworkClientBase DecoratedClient { get; }

		/// <summary>
		/// The encryption service provider for the connection.
		/// </summary>
		private ICryptoServiceProvider EncryptionServiceProvider { get; }

		/// <summary>
		/// The decryption service provider for the connection.
		/// </summary>
		private ICryptoServiceProvider DecryptionServiceProvider { get; }

		/// <summary>
		/// Creates a new crypto decorator for the <see cref="NetworkClientBase"/>.
		/// Extends the <see cref="ReadAsync"/> and <see cref="WriteAsync"/> implementations to pass
		/// all bytes through the corresponding incoming and outgoing ciphers.
		/// </summary>
		/// <param name="decoratedClient">The client to decorate.</param>
		/// <param name="encryptionServiceProvider">The encryption service.</param>
		/// <param name="decryptionServiceProvider">The decryption service.</param>
		public NetworkClientBlocklessCryptoDecorator(NetworkClientBase decoratedClient, ICryptoServiceProvider encryptionServiceProvider, ICryptoServiceProvider decryptionServiceProvider)
		{
			if(decoratedClient == null) throw new ArgumentNullException(nameof(decoratedClient));
			if(encryptionServiceProvider == null) throw new ArgumentNullException(nameof(encryptionServiceProvider));
			if(decryptionServiceProvider == null) throw new ArgumentNullException(nameof(decryptionServiceProvider));

			DecoratedClient = decoratedClient;
			EncryptionServiceProvider = encryptionServiceProvider;
			DecryptionServiceProvider = decryptionServiceProvider;
		}

		public override Task<bool> ConnectAsync(string ip, int port)
		{
			return DecoratedClient.ConnectAsync(ip, port)
;
		}

		/// <inheritdoc />
		public override Task ClearReadBuffers()
		{
			return DecoratedClient.ClearReadBuffers();
		}

		/// <inheritdoc />
		public override Task DisconnectAsync(int delay)
		{
			return DecoratedClient.DisconnectAsync(delay);
		}

		//TODO: Refactor this
		/// <summary>
		/// Reads asyncronously <see cref="count"/> many bytes from the reader.
		/// </summary>
		/// <param name="buffer">The buffer to store the bytes into.</param>
		/// <param name="start">The start position in the buffer to start reading into.</param>
		/// <param name="count">How many bytes to read.</param>
		/// <param name="token">The cancel token to check during the async operation.</param>
		/// <returns>A future for the read bytes.</returns>
		public override async Task<int> ReadAsync(byte[] buffer, int start, int count, CancellationToken token)
		{
			if(buffer == null) throw new ArgumentNullException(nameof(buffer));
			if(start < 0) throw new ArgumentOutOfRangeException(nameof(start));
			if(count < 0) throw new ArgumentOutOfRangeException(nameof(count), $"Cannot read less than 0 bytes. Can't read: {count} many bytes");
			if(buffer.Length < count) throw new ArgumentException($"Buffer must have a length that is at least the Count: {count}.", nameof(buffer));

			bool result = await ReadAndDecrypt(buffer, start, count, token)
				.ConfigureAwait(false);

			if(!result)
				return 0;

			return count;
		}

		private async Task<bool> ReadAndDecrypt(byte[] buffer, int start, int count, CancellationToken token)
		{
			//We don't need to know the amount read, I think.
			await DecoratedClient.ReadAsync(buffer, start, count, token)
				.ConfigureAwait(false);

			//Check cancel again, we want to fail quick
			if(token.IsCancellationRequested)
				return false;

			//We throw above if we have an invalid size that can't be decrypted once read.
			//That means callers will need to be careful in what they request to read.
			DecryptionServiceProvider.Crypt(buffer, start, count);

			//Check cancel again, we want to fail quick
			if(token.IsCancellationRequested)
				return false;

			return true;
		}

		public override Task WriteAsync(byte[] bytes, int offset, int count)
		{
			if(offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
			if(count < 0) throw new ArgumentOutOfRangeException(nameof(count));

			return DecoratedClient.WriteAsync(EncryptionServiceProvider.Crypt(bytes, offset, count), offset, count);
		}
	}
}
