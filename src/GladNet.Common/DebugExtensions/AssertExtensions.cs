using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet
{
	public static class AssertExtensions
	{
		/// <summary>
		/// Throws if the object is null.
		/// </summary>
		/// <typeparam name="TObjectType"></typeparam>
		/// <param name="instance">Instance to check.</param>
		/// <param name="paramName">Parameter name.</param>
		public static void ThrowIfNull<TObjectType>(this TObjectType instance, string paramName)
		{
			if (instance == null)
				throw new ArgumentNullException(paramName, "Type: " + typeof(TObjectType).ToString() + " was null as Parameter: " + paramName + ".");
		}

		/// <summary>
		/// Throws if the object is null.
		/// </summary>
		/// <typeparam name="TObjectType"></typeparam>
		/// <param name="instance">Instance to check.</param>
		public static void ThrowIfNull<TObjectType>(this TObjectType instance)
		{
			if (instance == null)
				throw new ArgumentNullException("Type: " + typeof(TObjectType).ToString() + " was null.");
		}
	}
}
