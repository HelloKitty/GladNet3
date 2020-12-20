using System;
using System.Collections.Generic;
using System.Text;

namespace GladNet
{
	/// <summary>
	/// Contract for types that are bindable to a <see cref="ITypeBinder{TBindTargetType,TConstraintType}"/>.
	/// </summary>
	/// <typeparam name="TBindTargetType"></typeparam>
	/// <typeparam name="TConstraintType"></typeparam>
	public interface ITypeBindable<out TBindTargetType, out TConstraintType> 
		where TBindTargetType : ITypeBindable<TBindTargetType, TConstraintType>
	{
		/// <summary>
		/// Binds this bindable to the specified <see cref="bindTarget"/>.
		/// </summary>
		/// <param name="bindTarget">The bind target to bind to.</param>
		void BindTo(ITypeBinder<TBindTargetType, TConstraintType> bindTarget);
	}
}
