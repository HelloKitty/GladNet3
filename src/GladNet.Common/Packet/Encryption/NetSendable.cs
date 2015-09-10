using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using GladNet.Common.NetSendable;

namespace GladNet.Common
{
	namespace NetSendable
	{
		public enum State : byte
		{
			Default,
			Serialized,
			Encrypted,
		}
	}

	/// <summary>
	/// Represents a wire-ready version of the TData that provides functionality to serialize, encrypt and decrypt the TData
	/// </summary>
	/// <typeparam name="TData">The Type of encryptable and serializable data becoming wire-ready.</typeparam>
	public class NetSendable<TData> : IEncryptable, ISerializable
		where TData : class
	{
		public State DataState { get; private set; }

		//This should never be serialized over the network.
		/// <summary>
		/// The TData to be manipulated or the resulting deserialized TData.
		/// </summary>
		public TData Data { get; private set; }

		//This should be serialized over the network.
		/// <summary>
		/// The wire-ready byte[] that represents the TData in the corressponding <see cref="State"/>
		/// </summary>
		private byte[] byteData = null;

		/// <summary>
		/// Constructor for typical <see cref="NetSendable"/> that requires an instance of TData.
		/// </summary>
		/// <param name="data">Instance of TData to be wire-ready prepared.</param>
		public NetSendable(TData data)
		{
			if (data == null)
				throw new ArgumentNullException("data", "TData data cannot be null in construction.");

			Data = data;
			DataState = State.Default;
		}


		/// <summary>
		/// Empty protobuf-net constuctor
		/// </summary>
		protected NetSendable()
		{

		}

		/// <summary>
		/// Encrypts the TData contained within this <see cref="NetSendable"/>.
		/// </summary>
		/// <param name="encryptor">Object responsible for the encryption.</param>
		/// <exception cref="InvalidOperationException">Throws when the <see cref="NetSendable"/> is not in a Serialized <see cref="NetSendable.State"/></exception>
		/// <returns></returns>
		public bool Encrypt(IEncryptor encryptor)
		{
			if (encryptor == null)
				throw new ArgumentNullException("encryptor", "The encryptor cannot be null.");

			if (DataState != State.Serialized)
				throw new InvalidOperationException(GetType() + " was not in a valid state for encryption. Must be serialized first.");

			try
			{
				byteData = encryptor.Encrypt(byteData);
			}
			catch (CryptographicException e)
			{
#if DEBUG || DEBUGBUILD
				//TODO: Logging.
				//We rethrow in debug builds
				throw;
#else
				return false;
#endif
			}

			//Check the state of the bytes
			if (byteData == null)
				return false;

			//If sucessful the data should be in an encrypted state.
			DataState = State.Encrypted;
			return true;
		}

		/// <summary>
		/// Decrypts the TData contained within this <see cref="NetSendable"/>
		/// </summary>
		/// <param name="decryptor"></param>
		/// <exception cref="InvalidOperationException">Throws when the <see cref="NetSendable"/> is not in a Encrypted <see cref="NetSendable.State"/>
		/// or if the internal byte representation is null..</exception>
		/// <returns></returns>
		public bool Decrypt(IDecryptor decryptor)
		{

			if (decryptor == null)
				throw new ArgumentNullException("decryptor", "The decryptor cannot be null.");

			if(DataState != State.Encrypted)
				throw new InvalidOperationException(GetType() + " was not in a valid state for decryption. Must be encrypted first.");

			if (byteData == null)
				throw new InvalidOperationException(GetType() + " was in an invalid state for decryption. Must have a non-null byteData representation.");

			try
			{
				byteData = decryptor.Decrypt(byteData);
			}
			catch(CryptographicException e)
			{
#if DEBUG || DEBUGBUILD
				//TODO: Logging.
				//We rethrow in debug builds
				throw;
#else
				return false;
#endif
			}

			//Check the state of the bytes
			if (byteData == null)
				return false;

			//If successful the data should be in a serialized state.
			DataState = State.Serialized;
			return true;
		}

		/// <summary>
		/// Serializes the TData into a byte[] for the wire.
		/// </summary>
		/// <param name="serializer">Serializer object for the serialization process.</param>
		/// <exception cref="InvalidOperationException">Throws if the data is not in Default <see cref="State"/></exception>
		/// <returns></returns>
		public bool Serialize(ISerializer serializer)
		{
			if (serializer == null)
				throw new ArgumentNullException("serializer", "The serializer cannot be null.");

			if (DataState != State.Default)
				throw new InvalidOperationException(GetType() + " was not in a valid state for serialization. Must be in default state.");

			byteData = serializer.Serialize(Data);

			//Check the state of the bytes
			if (byteData == null)
				return false;

			//If successful the data should be in a serialized state
			DataState = State.Serialized;
			return true;
		}

		/// <summary>
		/// Deserializes the byteData into TData.
		/// </summary>
		/// <param name="deserializer">Deserializer object for the deserialization process.</param>
		/// <exception cref="InvalidOperationException">Throws if the <see cref="State"/> isn't Serialized.</exception>
		/// <returns></returns>
		public bool Deserialize(IDeserializer deserializer)
		{
			if (deserializer == null)
				throw new ArgumentNullException("deserializer", "The derserializer cannot be null.");

			if (DataState != State.Serialized)
				throw new InvalidOperationException(GetType() + " was not in a valid state for deserialization. Must be in a serialized state.");

			Data = deserializer.Deserialize<TData>(byteData);

			if (Data == null)
				return false;

			DataState = State.Default;
			return true;
		}
	}
}
