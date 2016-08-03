using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GladNet.Serializer
{
	/// <summary>
	/// Implementer provides deserialization services.
	/// </summary>
	public interface IDeserializerStrategy
	{
		/// <summary>
		/// Deserializes the potential <typeparamref name="TData"/> instance represented by the <see cref="byte"/>[].
		/// </summary>
		/// <typeparam name="TData">Type to deserialize to.</typeparam>
		/// <param name="value">Byte representation of an object.</param>
		/// <returns>An instance/value deserialized from the given byte[] or null if failed.</returns>
		TData Deserialize<TData>(byte[] value);

		/// <summary>
		/// Deserializes the potential <typeparamref name="TData"/> instance represented by the <see cref="byte"/>[].
		/// </summary>
		/// <typeparam name="TData">Type to deserialize to.</typeparam>
		/// <param name="stream">Stream representation of an object.</param>
		/// <returns>An instance/value deserialized from the given byte[] or null if failed.</returns>
		TData Deserialize<TData>(Stream stream);
	}
}
