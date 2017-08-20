using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;


namespace GladNet.Encryption
{
	/// <summary>
	/// Creates a contract that an implementing type is capable of encrypting a <see cref="byte"/>[]
	/// Implementer is an encryption strategy.
	/// </summary>
	public interface IEncryptorStrategy
	{
		/// <summary>
		/// Encrypts a <see cref="byte"/>[].
		/// Should throw on failure.
		/// </summary>
		/// <param name="value">Chunk to be encrypted.</param>
		/// <exception cref="CryptographicException">Throws if the underlying encryption fails</exception>
		/// <returns>Returns null if valu parameter is null. Otherwise returns encrypted <see cref="byte"/>[]. Can throw if fails.</returns>
		byte[] Encrypt(byte[] value);
	}
}
