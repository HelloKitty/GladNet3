using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	/// <summary>
	/// Implementer is an object that can be decrypted.
	/// </summary>
	public interface IDecryptable
	{
		/// <summary>
		/// Attempts to decrypt the raw data within the implementing type.
		/// </summary>
		/// <param name="decryptor">The decryptor object that handles the specifics of encryption.</param>
		/// <exception cref="CryptographicException">Throws if decryption fails.</exception>
		/// <returns>Indicates if decryption was successful.</returns>
		bool Decrypt(IDecryptorStrategy decryptor);
	}
}
