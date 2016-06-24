using GladNet.Serializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GladNet.Serializer.Protobuf
{
	/// <summary>
	/// Protobuf-net serializer strategy.
	/// </summary>
	public class ProtobufnetSerializerStrategy : ISerializerStrategy
	{
		/// <summary>
		/// Serializes the a <typeparamref name="TData"/> instance/value to a <see cref="byte"/>[] using Protobuf-net.
		/// </summary>
		/// <typeparam name="TData">Type being serialized.</typeparam>
		/// <param name="data">Instance/value to serialize.</param>
		/// <returns>An instance/value serialized from the given instance of <typeparamref name="TData"/>.</returns>
		public byte[] Serialize<TData>(TData data)
		{
			if (data == null)
				throw new ArgumentNullException(nameof(data), $"Provided {data} is a null arg.");

			using (MemoryStream ms = new MemoryStream())
			{
				ProtoBuf.Serializer.Serialize(ms, data);

				return ms.ToArray();
			}
		}
	}
}
