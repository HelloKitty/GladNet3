using GladNet.Serializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GladNet.Serialization
{
	public class ProtobufnetSerializerStrategy : ISerializerStrategy
	{
		public byte[] Serialize<TData>(TData data)
		{
			data.ThrowIfNull(nameof(data));

			using (MemoryStream ms = new MemoryStream())
			{
				ProtoBuf.Serializer.Serialize(ms, data);

				return ms.ToArray();
			}
		}
	}
}
