using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	/// <summary>
	/// Contract for implementing objects to provide functionality to shallow copy an object.
	/// For information about shallow vs deep copying see: http://stackoverflow.com/questions/184710/what-is-the-difference-between-a-deep-copy-and-a-shallow-copy
	/// </summary>
	public interface IShallowCloneable
	{
		/// <summary>
		/// Creates a shallow copied instance of an object.
		/// </summary>
		/// <returns>Returns a non-null shallow copied instance of an object.</returns>
		object ShallowClone();
	}

	/// <summary>
	/// Contract for implementing objects to provide functionality to shallow copy an object.
	/// For information about shallow vs deep copying see: http://stackoverflow.com/questions/184710/what-is-the-difference-between-a-deep-copy-and-a-shallow-copy
	/// </summary>
	/// <typeparam name="TObjectType">Type copying.</typeparam>
	public interface IShallowCloneable<out TObjectType> : IShallowCloneable
	{

		/// <summary>
		/// Creates a shallow copied instance of an object.
		/// </summary>
		/// <returns>Returns a non-null shallow copied instance of an object.</returns>
		/// Hiding is intended
		new TObjectType ShallowClone();
	}
}
