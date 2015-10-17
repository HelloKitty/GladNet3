using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace GladNet.Common
{	
	/// <summary>
	/// Finite valid states a <see cref="NetSendable"/> can be in.
	/// </summary>
	[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")] //We suppress this because this is going over the wire. 1 byte is far better.
	public enum NetSendableState : byte
	{
		Default,
		Serialized,
		Encrypted,
	}

	/// <summary>
	/// Represents a wire-ready version of the TData that provides functionality to serialize, encrypt and decrypt the TData
	/// </summary>
	/// <typeparam name="TData">The Type of encryptable and serializable data becoming wire-ready.</typeparam>
	public class NetSendable<TData> : IEncryptable, ISerializable, IShallowCloneable<NetSendable<TData>>
		where TData : class
	{
		/// <summary>
		/// Indicates the state the object is currently in.
		/// </summary>
		public NetSendableState DataState { get; private set; }

		//This should never be serialized over the network.
		/// <summary>
		/// The TData to be manipulated or the resulting deserialized TData. Can be null depending on state.
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
			DataState = NetSendableState.Default;
		}


		//Protobuf-net constructors should be marked with ncrunch no coverage to suppress it from coverage metrics
		//ncrunch: no coverage start
		/// <summary>
		/// Empty protobuf-net constuctor
		/// </summary>
		protected NetSendable()
		{

		}
		//ncrunch: no coverage end

		/// <summary>
		/// Encrypts the TData contained within this <see cref="NetSendable"/>.
		/// </summary>
		/// <param name="encryptor">Object responsible for the encryption.</param>
		/// <exception cref="InvalidOperationException">Throws when the <see cref="NetSendable"/> is not in a Serialized <see cref="NetSendableState"/></exception>
		/// <returns>Indicates if encryption was successful</returns>
		public bool Encrypt(IEncryptor encryptor)
		{
			if (encryptor == null)
				throw new ArgumentNullException("encryptor", "The encryptor cannot be null.");

			ThrowIfInvalidState(NetSendableState.Serialized, false);
			
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
			DataState = NetSendableState.Encrypted;
			return true;
		}

		/// <summary>
		/// Decrypts the TData contained within this <see cref="NetSendable"/>
		/// </summary>
		/// <param name="decryptor"></param>
		/// <exception cref="InvalidOperationException">Throws when the <see cref="NetSendable"/> is not in a Encrypted <see cref="NetSendableState"/>
		/// or if the internal byte representation is null..</exception>
		/// <returns>Indicates if decryption was successful.</returns>
		public bool Decrypt(IDecryptor decryptor)
		{

			if (decryptor == null)
				throw new ArgumentNullException("decryptor", "The decryptor cannot be null.");

			ThrowIfInvalidState(NetSendableState.Encrypted, true);

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
			DataState = NetSendableState.Serialized;
			return true;
		}

		/// <summary>
		/// Serializes the TData into a byte[] for the wire.
		/// </summary>
		/// <param name="serializer">Serializer object for the serialization process.</param>
		/// <exception cref="InvalidOperationException">Throws if the data is not in Default <see cref="State"/></exception>
		/// <returns>Inidicates if serialization was successful</returns>
		public bool Serialize(ISerializer serializer)
		{
			if (serializer == null)
				throw new ArgumentNullException("serializer", "The serializer cannot be null.");

			ThrowIfInvalidState(NetSendableState.Default, false);

			byteData = serializer.Serialize(Data);

			//Check the state of the bytes
			if (byteData == null)
				return false;

			//If successful the data should be in a serialized state
			DataState = NetSendableState.Serialized;

			//We don't need Data anymore and it can be recreated from the current state now.
			Data = null;
			return true;
		}

		/// <summary>
		/// Deserializes the byteData into TData.
		/// </summary>
		/// <param name="deserializer">Deserializer object for the deserialization process.</param>
		/// <exception cref="InvalidOperationException">Throws if the <see cref="State"/> isn't Serialized.</exception>
		/// <returns>Indicates if deserialization was successful</returns>
		public bool Deserialize(IDeserializer deserializer)
		{
			if (deserializer == null)
				throw new ArgumentNullException("deserializer", "The derserializer cannot be null.");

			ThrowIfInvalidState(NetSendableState.Serialized, false);

			Data = deserializer.Deserialize<TData>(byteData);

			if (Data == null)
				return false;

			DataState = NetSendableState.Default;
			return true;
		}

		[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "byteData")]
		private void ThrowIfInvalidState(NetSendableState expectedState, bool checkData)
		{
			if(DataState != expectedState)
				throw new InvalidOperationException(GetType() + " was not in required state " + expectedState + " was in " + DataState);

			if (checkData && byteData == null)
				throw new InvalidOperationException(GetType() + " was in an invalid state for " + expectedState + ". Must have a non-null byteData representation.");
		}

		public NetSendable<TData> ShallowClone()
		{
			//As of Oct. 8th 2015 it is valid to MemberwiseClone for valid ShallowCopy.
			return MemberwiseClone() as NetSendable<TData>; //it never shouldn't be of this type
		}

		object IShallowCloneable.ShallowClone()
		{
			return this.ShallowClone();
		}
	}
}
