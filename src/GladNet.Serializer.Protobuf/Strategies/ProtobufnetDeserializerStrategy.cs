using GladNet.Serializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GladNet.Serializer.Protobuf
{
	public class ProtobufnetDeserializerStrategy : IDeserializerStrategy
	{
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
