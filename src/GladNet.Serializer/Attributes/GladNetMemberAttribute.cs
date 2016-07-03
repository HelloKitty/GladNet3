using Easyception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Serializer
{
	//Based on: https://github.com/mgravell/protobuf-net/blob/e601b359c6ae56afc159754d29f5e7d0f05a01f5/protobuf-net/ProtoMemberAttribute.cs
	//GladNet serialization is loosely based on Protobuf-net requirements/specs. Any other support is secondary and up to furture implementors
	//to work with information provided.
	/// <summary>
	/// Marks members with Meta-data about serialization. Existence indicates the member is a target
	/// that requires serialization. Optional information can be included to better serialize.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
		AllowMultiple = false, Inherited = true)]
	public sealed class GladNetMemberAttribute : Attribute
	{
		/// <summary>
		/// Indicates whether this member is mandatory.
		/// </summary>
		public bool IsRequired { get; set; }

		/// <summary>
		/// Inidicates the unique (unenforced) ID of the member for the <see cref="Type"/> to be
		/// serialized.
		/// </summary>
		public int TagID { get; private set; }

		/// <summary>
		/// Create a new member attribute for a target data.
		/// </summary>
		/// <param name="tagID">A positive integer (non-zero)</param>
		/// An integer from Z^+: http://mathworld.wolfram.com/Z-Plus.html </param>
		/// <exception cref="ArgumentOutOfRangeException">Throws if tagID is 0 or negative.</exception>
		[Obsolete("This has been marked obsolete because we need to abstract the indicies of the payload data slots. The reason is we must not collide with internally defined slots that may come to exist in the future.", true)]
		public GladNetMemberAttribute(int tagID)
		{
			//uint is not CLS compliant. We have no reason to use uint in .Net
			if (tagID <= 0)
				throw new ArgumentOutOfRangeException("tagID", "tagID must be a positive non-zero integer. See revelant documentation.");

			TagID = tagID;
		}

		/// <summary>
		/// Create a new member attribute for a target data.
		/// </summary>
		/// <param name="dataIndex">A valid unique <see cref="GladNetPayloadDataIndex"/> for this type.</param>
		/// <exception cref="ArgumentException">Throws if <paramref name="dataIndex"/> is outside of the range of valid <see cref="GladNetPayloadDataIndex"/>s.</exception>
		public GladNetMemberAttribute(GladNetPayloadDataIndex dataIndex)
		{
			//Check if it's defined. Users may try to cheat the system by casting an int
			//Me must assert that this is an issue as if we don't they'll encounter odd issues in the future when
			//it collides with internal indicies
			Throw<ArgumentException>.If.IsTrue(!Enum.IsDefined(typeof(GladNetPayloadDataIndex), dataIndex));

			TagID = (int)dataIndex;
		}
	}
}
