using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GladNet.Serializer
{

	//Attribute usage based on: https://github.com/mgravell/protobuf-net/blob/e601b359c6ae56afc159754d29f5e7d0f05a01f5/protobuf-net/ProtoIncludeAttribute.cs
	//GladNet uses Protobuf-net by default so the abstraction is loosely based on those specs.
	/// <summary>
	/// Implies a (unenforced) contract between a <see cref="GladNetSerializationContractAttribute"/> marked construct
	/// and another <see cref="Type"/> that is a derived or base type the targeted <see cref="Type"/>. 
	/// Meta-data needed to map an int to a <see cref="Type"/>
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, 
		AllowMultiple = true, Inherited = false)]
	public class GladNetSerializationIncludeAttribute : Attribute
	{
		/// <summary>
		/// Indicates the <see cref="Type"/> of the inheriting/implementing
		/// construct. Used to identify objects in case of less-derived Type serialized.
		/// </summary>
		public Type TypeToWireTo { get; private set; }

		/// <summary>
		/// Indiciates a unique (not enforced) ID for that can be used to map
		/// int -> Type for derived types.
		/// </summary>
		public int TagID { get; private set; }

		public bool IncludeForDerived { get; private set; }

		/// <summary>
		/// Marks a target with the Include attribute. 
		/// Attribute doesn't handle enforcing.
		/// </summary>
		/// <param name="tagID">The unique (unenforced) ID to associate with the <see cref="Type"/>.</param>
		/// <param name="type">Type of the derived or base Type to associate with. (Unenforced to be a derived or subtype)</param>
		/// <exception cref="ArgumentOutOfRangeException">Throws if tagID is 0 or negative.</exception>
		public GladNetSerializationIncludeAttribute(int tagID, Type type, bool isForDerived = true)
			: base()
		{
			//uint is not CLS compliant. We have no reason to use uint in .Net
			if (tagID <= 1) throw new ArgumentOutOfRangeException(nameof(tagID), "tagID must be a positive greater than 1 integer. 1 is reserved. See revelant documentation.");
			if (type == null) throw new ArgumentNullException(nameof(type), $"DerivedType cannot be null in {nameof(GladNetSerializationIncludeAttribute)}.ctor(...).");

			IncludeForDerived = isForDerived;
			TypeToWireTo = type;
			TagID = tagID;
		}
	}
}
