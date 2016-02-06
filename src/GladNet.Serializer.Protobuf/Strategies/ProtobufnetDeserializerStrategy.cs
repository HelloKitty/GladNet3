using GladNet.Serializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GladNet.Serializer.Protobuf
{
	/// <summary>
	/// Protobuf-net deserializer strategy.
	/// </summary>
	public class ProtobufnetDeserializerStrategy : IDeserializerStrategy
	{
		/// <summary>
		/// Deserializes the potential <typeparamref name="TData"/> instance represented by the <see cref="byte"/>[] using Protobuf-net.
		/// </summary>
		/// <typeparam name="TData">Type to deserialize to.</typeparam>
		/// <param name="value">Byte representation of an object.</param>
		/// <returns>An instance/value deserialized from the given byte[] or null if failed.</returns>
		public TData Deserialize<TData>(byte[] value)
		{
			using (MemoryStream ms = new MemoryStream(value))
			{
				TData data = ProtoBuf.Serializer.Deserialize<TData>(ms);

				data.ThrowIfNull(nameof(data));

				return data;
            }
		}
	}
}
