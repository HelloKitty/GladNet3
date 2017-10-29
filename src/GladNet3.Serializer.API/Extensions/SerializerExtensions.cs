using System;
using System.Collections.Generic;
using System.Text;

namespace GladNet
{
	public static class SerializerExtensions
	{
		//We shouldn't expect the deserialize to provide always non-null values.
		//That is a serialization implementation detail.
		/// <summary>
		/// Attempts to deserialize to <typeparamref name="TTypeToDeserializeTo"/> from the provided <see cref="byte[]"/>.
		/// </summary>
		/// <typeparam name="TTypeToDeserializeTo"></typeparam>
		/// <param name="serializer"></param>
		/// <param name="data">Byte repsentation of <typeparamref name="TTypeToDeserializeTo"/>.</param>
		/// <returns>An instance of <typeparamref name="TTypeToDeserializeTo"/> or null if failed.</returns>
		public static TTypeToDeserializeTo Deserialize<TTypeToDeserializeTo>(this INetworkSerializationService serializer, byte[] data)
		{
			if(data == null) throw new ArgumentNullException(nameof(data));

			return serializer.Deserialize<TTypeToDeserializeTo>(data, 0, data.Length);
		}
	}
}
