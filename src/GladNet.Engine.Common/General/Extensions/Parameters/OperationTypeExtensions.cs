using GladNet.Common;
using GladNet.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Engine.Common
{
	public static class OperationTypeExtensions
	{
		/// <summary>
		/// Maps <see cref="OperationType"/> to <see cref="Type"/> of <see cref="NetworkMessage"/>.
		/// </summary>
		/// <param name="opType"><see cref="OperationType"/> to map.</param>
		/// <returns><see cref="Type"/> of <see cref="NetworkMessage"/> the <see cref="OperationType"/> maps to.</returns>
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
