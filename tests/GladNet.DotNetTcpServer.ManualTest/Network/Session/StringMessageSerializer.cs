using System;
using System.Collections.Generic;
using System.Text;

namespace GladNet
{
	public sealed class StringMessageSerializer : IMessageSerializer<string>, IMessageDeserializer<string>
	{
		public void Serialize(string value, Span<byte> buffer, ref int offset)
		{
			offset += Encoding.ASCII.GetBytes(value, buffer);
		}

		public string Deserialize(Span<byte> buffer, ref int offset)
		{
			string value = Encoding.ASCII.GetString(buffer);
			offset += Encoding.ASCII.GetByteCount(value);
			return value;
		}
	}
}
