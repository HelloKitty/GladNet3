using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public static class OperationTypeExtensions
	{
		public static Type ToNetworkMessageType(this OperationType opType)
		{
			switch(opType)
			{
				case OperationType.Event:
					return typeof(EventMessage);
				case OperationType.Request:
					return typeof(RequestMessage);
				case OperationType.Response:
					return typeof(ResponseMessage);

				default:
					throw new ArgumentOutOfRangeException("opType", "opType in was not within the valid range.");
			}
		}
	}
}
