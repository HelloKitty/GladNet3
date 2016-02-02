using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Serializer
{
	/// <summary>
	/// Implementer is an object that can be deserialized
	/// </summary>
	public interface IDeserializable
	{
		/// <summary>
		/// Attempts to deserialize an object with a given deserialization strategy.
		/// </summary>
		/// <param name="deserializer">Deserialization strategy.</param>
		/// <returns>Indicates if the deserialization was successful.</returns>
		bool Deserialize(IDeserializerStrategy deserializer);
	}
}
