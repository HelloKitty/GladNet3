using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Serializer
{
	//The reason this was added is because we MUST reserve indicies for future use
	//in GladNet2 internals but at the same time we must let users specify the index at compile time
	//in an attribute. To do this we require users to provide a GladNetDataIndex
	/// <summary>
	/// Indicates the slot to use for data member serialization.
	/// </summary>
	public enum GladNetDataIndex : int
	{
		//TODO: Create more of these... Lots more.
		Index1 = GladNetDataIndexInternal.LastInternalSlot + 1,
		Index2 = GladNetDataIndexInternal.LastInternalSlot + 2,
		Index3 = GladNetDataIndexInternal.LastInternalSlot + 3,
		Index4 = GladNetDataIndexInternal.LastInternalSlot + 4,
		Index5 = GladNetDataIndexInternal.LastInternalSlot + 5,
		Index6 = GladNetDataIndexInternal.LastInternalSlot + 6,
		Index7 = GladNetDataIndexInternal.LastInternalSlot + 7,
		Index8 = GladNetDataIndexInternal.LastInternalSlot + 8,
		Index9 = GladNetDataIndexInternal.LastInternalSlot + 9,
		Index10 = GladNetDataIndexInternal.LastInternalSlot + 10,
		Index11 = GladNetDataIndexInternal.LastInternalSlot + 11,
		Index12 = GladNetDataIndexInternal.LastInternalSlot + 12,
		Index13 = GladNetDataIndexInternal.LastInternalSlot + 13,
		Index14 = GladNetDataIndexInternal.LastInternalSlot + 14,
		Index15 = GladNetDataIndexInternal.LastInternalSlot + 15,
		Index16 = GladNetDataIndexInternal.LastInternalSlot + 16,
		Index17 = GladNetDataIndexInternal.LastInternalSlot + 17,
		Index18 = GladNetDataIndexInternal.LastInternalSlot + 18,
		Index19 = GladNetDataIndexInternal.LastInternalSlot + 19,
		Index20 = GladNetDataIndexInternal.LastInternalSlot + 20,
		Index21 = GladNetDataIndexInternal.LastInternalSlot + 21,
		Index22 = GladNetDataIndexInternal.LastInternalSlot + 22,
		Index23 = GladNetDataIndexInternal.LastInternalSlot + 23,
		Index24 = GladNetDataIndexInternal.LastInternalSlot + 24,
		Index25 = GladNetDataIndexInternal.LastInternalSlot + 25,
		Index26 = GladNetDataIndexInternal.LastInternalSlot + 26,
		Index27 = GladNetDataIndexInternal.LastInternalSlot + 27,
		Index28 = GladNetDataIndexInternal.LastInternalSlot + 28,
		Index29 = GladNetDataIndexInternal.LastInternalSlot + 29,
		Index30 = GladNetDataIndexInternal.LastInternalSlot + 30,
		Index31 = GladNetDataIndexInternal.LastInternalSlot + 31,
		Index32 = GladNetDataIndexInternal.LastInternalSlot + 32,
		Index33 = GladNetDataIndexInternal.LastInternalSlot + 33,
		Index34 = GladNetDataIndexInternal.LastInternalSlot + 34,
		Index35 = GladNetDataIndexInternal.LastInternalSlot + 35,
		Index36 = GladNetDataIndexInternal.LastInternalSlot + 36,
		Index37 = GladNetDataIndexInternal.LastInternalSlot + 37,
		Index38 = GladNetDataIndexInternal.LastInternalSlot + 38,
	}

	/// <summary>
	/// For GladNet2 internal use only. Uses to internally mark members of a Type as
	/// serializable members.
	/// </summary>
	internal enum GladNetDataIndexInternal : int
	{
		/// <summary>
		/// This Enum field indicates the last internal reserved value for data indicies.
		/// </summary>
		LastInternalSlot = 1,
	}
}
