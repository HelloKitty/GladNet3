using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Serializer
{
	/// <summary>
	/// Implementer is an object that can be serialized by a visitor.
	/// </summary>
	public interface ISerializationVisitable
	{
		/// <summary>
		/// Attempts to serialize an object with a given serialization strategy.
		/// </summary>
		/// <param name="serializer">Serialization strategy.</param>
		/// <returns>Returns a non-null array of bytes representing the serialized object or throws.</returns>
		byte[] SerializeWithVisitor(ISerializerStrategy serializer);
	}
}
