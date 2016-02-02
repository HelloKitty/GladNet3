using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GladNet.Common
{
	/// <summary>
	/// Creates a contract that an implementing type is capable of decrypting a <see cref="byte[]"/>
	/// No promise is made that the implementing type can decrypt data encrypted by any algorithm.
	/// Implementer is a decryption strategy.
	/// </summary>
	public interface IDecryptorStrategy
	{
		/// <summary>
		/// Decrypts a <see cref="byte[]"/> that is encrypted if possible.
		/// Should throw on failure.
		/// </summary>
		/// <param name="value">Chunk to be decrypted.</param>
		/// <exception cref="CryptographicException">Throws if the underlying decryption fails</exception>
		/// <returns>Returns null if valu parameter is null. Otherwise returns decrypted <see cref="byte[]"/>. Can throw if fails.</returns>
		byte[] Decrypt(byte[] value);
	}
}
