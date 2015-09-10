using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public interface IDeserializer
	{
		TData Deserialize<TData>(byte[] bytes);
	}
}
