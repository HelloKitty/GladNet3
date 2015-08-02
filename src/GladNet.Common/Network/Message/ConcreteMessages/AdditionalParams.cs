using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	public class AdditionalParams
	{
		public readonly byte ByteCode;

		public readonly string Message;

		public AdditionalParams(byte code, string message)
		{
			ByteCode = code;
			Message = message;
		}
	}
}
