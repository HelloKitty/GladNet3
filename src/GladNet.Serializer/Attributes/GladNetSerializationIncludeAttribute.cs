using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Serializer
{

	//Attribute usage based on: https://github.com/mgravell/protobuf-net/blob/e601b359c6ae56afc159754d29f5e7d0f05a01f5/protobuf-net/ProtoIncludeAttribute.cs
	//GladNet uses Protobuf-net by default so the abstraction is loosely based on those specs.
	/// <summary>
	/// Implies a (unenforced) contract between a <see cref="GladNetSerializationContractAttribute"/> marked construct
	/// and another <see cref="Type"/> that derives/implements the targeted <see cref="Type"/>. 
	/// Meta-data needed to map an int to a <see cref="Type"/>
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, 
		AllowMultiple = true, Inherited = false)]
	public sealed class GladNetSerializationIncludeAttribute : Attribute
	{
		/// <summary>
		/// Indicates the <see cref="Type"/> of the inheriting/implementing
		/// construct. Used to identify objects in case of less-derived Type serialized.
		/// </summary>
		public Type DerivedTypeToInclude { get; private set; }

		/// <summary>
		/// Indiciates a unique (not enforced) ID for that can be used to map
		/// int -> Type for derived types.
		/// </summary>
		public int TagID { get; private set; }

		/// <summary>
		/// Marks a target with the Include attribute. 
		/// Attribute doesn't handle enforcing.
		/// </summary>
		/// <param name="tagID">The unique (unenforced) ID to associate with the derived <see cref="Type"/>.</param>
		/// <param name="derivedType">Type of the derived Type to associate with. (Unenforced to be a derived type)</param>
		/// <exception cref="ArgumentOutOfRangeException">Throws if tagID is 0 or negative.</exception>
		public GladNetSerializationIncludeAttribute(int tagID, Type derivedType)
			: base()
		{
			//uint is not CLS compliant. We have no reason to use uint in .Net
			if (tagID <= 0)
				throw new ArgumentOutOfRangeException("tagID", "tagID must be a positive non-zero integer. See revelant documentation.");

			if (derivedType == null)
				throw new ArgumentNullException("derivedType", "DerivedType cannot be null in " + typeof(GladNetSerializationIncludeAttribute) + ".ctor(...).");

			DerivedTypeToInclude = derivedType;
			TagID = tagID;
		}
	}
}
