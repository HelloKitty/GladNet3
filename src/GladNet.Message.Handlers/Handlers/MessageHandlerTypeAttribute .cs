using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Message.Handlers
{
	/// <summary>
	/// Metadata marker for a message handler.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
	public class MessageHandlerTypeAttribute : Attribute
	{
		/// <summary>
		/// Operation Type the handler handles.
		/// </summary>
		public OperationType OpType { get; }

		public MessageHandlerTypeAttribute(OperationType opType)
		{
			OpType = opType;
		}
	}
}
