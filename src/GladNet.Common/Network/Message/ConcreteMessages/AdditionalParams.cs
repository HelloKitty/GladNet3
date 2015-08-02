using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	public class AdditionalParameters
	{
		private readonly byte _ByteCode;
		public byte ByteCode
		{
			get { return _ByteCode; }
		}

		private readonly string _Message;
		public string Message
		{
			get { return _Message; }
		}

		public AdditionalParameters(byte code, string message)
		{
			_ByteCode = code;
			_Message = message;
		}
	}
}
