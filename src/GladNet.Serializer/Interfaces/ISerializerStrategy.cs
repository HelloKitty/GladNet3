using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Serializer
{
	/// <summary>
	/// Implementer provides serialization services.
	/// </summary>
	public interface ISerializerStrategy
	{
		/// <summary>
		/// Serializes the a <typeparamref name="TData"/> instance/value to a byte[].
		/// </summary>
		/// <typeparam name="TData">Type being serialized.</typeparam>
		/// <param name="data">Instance/value to serialize.</param>
		/// <returns>An instance/value serialized from the given instance of <typeparamref name="TData"/>.</returns>
		byte[] Serialize<TData>(TData data);
	}
}
