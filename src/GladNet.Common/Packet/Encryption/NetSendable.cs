using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public class NetSendable<TData> : IEncryptable, ISerializable
		where TData : class
	{
		public enum State : byte
		{
			Default,
			Serialized,
			Encrypted,
		}

		public State DataState { get; protected set; }

		public TData Data { get; private set; }

		/// <summary>
		/// Encrypts the TData contained within this <see cref="NetSendable"/>.
		/// </summary>
		/// <param name="encryptor">Object responsible for the encryption.</param>
		/// <exception cref="InvalidOperationException">Throws when the <see cref="NetSendable"/> is not in a Serialized <see cref="NetSendable.State"/></exception>
		/// <returns></returns>
		public bool Encrypt(IEncryptor encryptor)
		{
			if (DataState != State.Serialized)
				throw new InvalidOperationException(GetType() + " was not in a valid state for encryption. Must be serialized first.");

			throw new NotImplementedException();

			//If sucessful the data should be in an encrypted state.
			DataState = State.Encrypted;
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="decryptor"></param>
		/// <exception cref="InvalidOperationException">Throws when the <see cref="NetSendable"/> is not in a Encrypted <see cref="NetSendable.State"/>.</exception>
		/// <returns></returns>
		public bool Decrypt(IDecryptor decryptor)
		{
			if(DataState != State.Encrypted)
				throw new InvalidOperationException(GetType() + " was not in a valid state for decryption. Must be encrypted first.");

			throw new NotImplementedException();

			//If successful the data should be in a serialized state.
			DataState = State.Serialized;
			return false;
		}

		public bool Serialize(ISerializer serializer)
		{
			if (DataState != State.Default)
				throw new InvalidOperationException(GetType() + " was not in a valid state for serialization. Must be in default state.");


			//If successful the data should be in a serialized state
			DataState = State.Serialized;
			return true;
		}

		public bool Deserialize(IDeserializer deserializer)
		{
			throw new NotImplementedException();
		}
	}
}
