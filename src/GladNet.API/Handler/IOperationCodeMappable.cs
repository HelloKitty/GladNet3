using System;
using System.Collections.Generic;
using System.Text;

namespace GladNet
{
	/// <summary>
	/// Contract for types that can be mapped to an operation code.
	/// </summary>
	/// <typeparam name="TOperationCodeType"></typeparam>
	public interface IOperationCodeMappable<out TOperationCodeType>
		where TOperationCodeType : Enum
	{
		/// <summary>
		/// The operation code value this mappable can be mapped to.
		/// </summary>
		TOperationCodeType OperationCode { get; }
	}
}
