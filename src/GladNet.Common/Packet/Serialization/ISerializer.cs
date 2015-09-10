using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public interface ISerializer
	{
		byte[] Serialize<TData>(TData data);
	}
}
