using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Message
{
	/// <summary>
	/// Object that can be mapped to an <see cref="OperationType"/>.
	/// </summary>
	public interface IOperationTypeMappable
	{
		/// <summary>
		/// Indicates the <see cref="OperationType"/> that this object maps to.
		/// </summary>
		OperationType OperationTypeMappedValue { get; }
	}
}
