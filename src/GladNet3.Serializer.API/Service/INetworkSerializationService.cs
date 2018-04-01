using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GladNet
{
	public interface INetworkSerializationService
	{
		//TODO: Should we provide a buffer to write into?
		/// <summary>
		/// Attempts to serialize the provided <paramref name="data"/>.
		/// </summary>
		/// <typeparam name="TTypeToSerialize">Type that is being serialized (can be inferred).</typeparam>
		/// <param name="data">Instance/value to serialize.</param>
		/// <returns>Byte array representation of the object.</returns>
		byte[] Serialize<TTypeToSerialize>(TTypeToSerialize data);

		//We shouldn't expect the deserialize to provide always non-null values.
		//That is a serialization implementation detail.
		/// <summary>
		/// Attempts to deserialize to <typeparamref name="TTypeToDeserializeTo"/> from the provided <see cref="byte[]"/>.
		/// </summary>
		/// <typeparam name="TTypeToDeserializeTo"></typeparam>
		/// <param name="buffer">Byte repsentation of <typeparamref name="TTypeToDeserializeTo"/>.</param>
		/// <param name="start"></param>
		/// <param name="count"></param>
		/// <returns>An instance of <typeparamref name="TTypeToDeserializeTo"/> or null if failed.</returns>
		TTypeToDeserializeTo Deserialize<TTypeToDeserializeTo>(byte[] buffer, int start, int count);

		//That is a serialization implementation detail.
		/// <summary>
		/// Attempts to deserialize to <typeparamref name="TTypeToDeserializeTo"/> from the provided <see cref="IBytesReadable"/>.
		/// </summary>
		/// <typeparam name="TTypeToDeserializeTo"></typeparam>
		/// <param name="bytesReadable">Byte readable object containing <typeparamref name="TTypeToDeserializeTo"/>.</param>
		/// <param name="token">Cancel token.</param>
		/// <returns>An instance of <typeparamref name="TTypeToDeserializeTo"/> or null if failed.</returns>
		[Obsolete("Most serializers do not support async deserialization. This feature may be removed in future versions.")]
		Task<TTypeToDeserializeTo> DeserializeAsync<TTypeToDeserializeTo>(IBytesReadable bytesReadable, CancellationToken token);
	}
}
