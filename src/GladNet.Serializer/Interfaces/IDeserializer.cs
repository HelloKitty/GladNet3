using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Serializer
{
	public interface IDeserializer
	{
		TData Deserialize<TData>(byte[] value);
	}
}
