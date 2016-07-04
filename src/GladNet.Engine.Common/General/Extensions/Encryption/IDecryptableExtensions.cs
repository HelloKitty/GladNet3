using GladNet.Encryption;
using GladNet.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Engine.Common
{
	public static class IDecryptableExtensions
	{
		/// <summary>
		/// Decrypts an object with the provided decryption strategy.
		/// </summary>
		/// <typeparam name="TObjectType">Type being decrypted.</typeparam>
		/// <param name="obj">Object being decrypted.</param>
		/// <param name="decryptor">Strategy for decryption.</param>
		/// <returns>A a decerypted deserializable instance.</returns>
		public static IDeserializable DecryptWith<TObjectType>(this TObjectType obj, IDecryptorStrategy decryptor)
			where TObjectType : IDecryptable, IDeserializable
		{
			//decrypt the object and return the object to fluently chain on
			obj.Decrypt(decryptor);

			return obj;
		}
	}
}
