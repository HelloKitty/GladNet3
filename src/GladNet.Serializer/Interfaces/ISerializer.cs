using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Serializer
{
	public interface ISerializer
	{
		byte[] Serialize<TData>(TData data);
	}
}
