using System;
using System.Collections.Generic;
using System.Text;

namespace GladNet
{
	/// <summary>
	/// Contract for types that can be mapped to many operation code values.
	/// Similar to <see cref="IOperationCodeMappable{TOperationCodeType}"/> but only for a single operation code.
	/// </summary>
	/// <typeparam name="TOperationCodeType"></typeparam>
	public interface IOperationCodesMappable<out TOperationCodeType>
		where TOperationCodeType : Enum
	{
		/// <summary>
		/// The operation code values this mappable can be mapped to.
		/// </summary>
		IEnumerable<TOperationCodeType> OperationCodes { get; }
	}
}
