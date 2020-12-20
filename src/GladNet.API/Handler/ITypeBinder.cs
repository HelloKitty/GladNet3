using System;
using System.Collections.Generic;
using System.Text;

namespace GladNet
{
	/// <summary>
	/// Contract for types that can have types bound to them.
	/// Binding TBindType to the specified TBindTargetType.
	/// </summary>
	/// <typeparam name="TBindTargetType">The object type to bind to.</typeparam>
	/// <typeparam name="TConstraintType">The constraining type.</typeparam>
	public interface ITypeBinder<in TBindTargetType, in TConstraintType>
		where TBindTargetType : ITypeBindable<TBindTargetType, TConstraintType>
	{
		/// <summary>
		/// Binds a bindable <see cref="bindable"/> instance to the specified <typeparamref name="TBindTargetType"/>.
		/// </summary>
		/// <typeparam name="TBindType"></typeparam>
		/// <param name="bindable">The bindable.</param>
		/// <returns>True if successfully bound.</returns>
		bool Bind<TBindType>(TBindTargetType bindable)
			where TBindType : TConstraintType;
	}
}
