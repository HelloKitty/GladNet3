using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	/// <summary>
	/// Contract for implementing objects to provide functionality to deep copy an object.
	/// For information about shallow vs deep copying see: http://stackoverflow.com/questions/184710/what-is-the-difference-between-a-deep-copy-and-a-shallow-copy
	/// </summary>
	public interface IDeepCloneable
	{
		object DeepClone();
	}

	/// <summary>
	/// Contract for implementing objects to provide functionality to deep copy an object.
	/// For information about shallow vs deep copying see: http://stackoverflow.com/questions/184710/what-is-the-difference-between-a-deep-copy-and-a-shallow-copy
	/// </summary>
	/// <typeparam name="TObjectType">Type copying.</typeparam>
	public interface IDeepCloneable<out TObjectType> : IDeepCloneable
	{
		//Hiding is intended
		new TObjectType DeepClone();
	}
}
