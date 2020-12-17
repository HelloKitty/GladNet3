using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GladNet
{
	/// <summary>
	/// Contract for types that implement serialization support for the network.
	/// </summary>
	public interface INetworkSerializationService
	{
		//TODO: Should we provide a buffer to write into?
		/// <summary>
		/// Attempts to serialize the provided <paramref name="data"/>.
		/// </summary>
		/// <typeparam name="TTypeToSerialize">Type that is being serialized (can be inferred).</typeparam>
		/// <param name="data">Instance/value to serialize.</param>
		/// <param name="buffer">Data buffer to write the provided object into.</param>
		/// <returns>Indicates how many bytes were written.</returns>
		int Serialize<TTypeToSerialize>(TTypeToSerialize data, Span<byte> buffer);

		//We shouldn't expect the deserialize to provide always non-null values.
		//That is a serialization implementation detail.
		/// <summary>
		/// Attempts to deserialize to <typeparamref name="TTypeToDeserializeTo"/> from the provided buffer.
		/// </summary>
		/// <typeparam name="TTypeToDeserializeTo"></typeparam>
		/// <param name="buffer">Binary buffer.</param>
		/// <returns>An instance of <typeparamref name="TTypeToDeserializeTo"/> or null if failed.</returns>
		TTypeToDeserializeTo Deserialize<TTypeToDeserializeTo>(Span<byte> buffer);
	}
}
