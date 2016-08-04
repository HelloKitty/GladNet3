using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladLive.Common
{
	/// <summary>
	/// Metadata marker for a payload handler.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class PayloadHandlerTypeAttribute : Attribute
	{
		/// <summary>
		/// Operation Type the handler handles.
		/// </summary>
		public OperationType OpType { get; }

		/// <summary>
		/// Types the handler can handle
		/// </summary>
		public Type[] PayloadTypes { get; }

		public PayloadHandlerTypeAttribute(OperationType opType, params Type[] types)
		{
			PayloadTypes = types;
			OpType = opType;
		}
	}
}
