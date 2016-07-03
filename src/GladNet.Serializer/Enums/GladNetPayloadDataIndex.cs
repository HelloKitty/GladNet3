using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Serializer
{
	//The reason this was added is because we MUST reserve indicies for future use
	//in GladNet2 internals but at the same time we must let users specify the index at compile time
	//in an attribute. To do this we require users to provide a GladNetPayloadDataIndex
	/// <summary>
	/// Indicates the slot to use for data member serialization.
	/// </summary>
	public enum GladNetPayloadDataIndex : int
	{
		//TODO: Create more of these... Lots more.
		Index1 = GladNetPayloadDataIndexInternal.LastInternalSlot + 1,
		Index2 = GladNetPayloadDataIndexInternal.LastInternalSlot + 2,
		Index3 = GladNetPayloadDataIndexInternal.LastInternalSlot + 3,
		Index4 = GladNetPayloadDataIndexInternal.LastInternalSlot + 4,
		Index5 = GladNetPayloadDataIndexInternal.LastInternalSlot + 5,
		Index6 = GladNetPayloadDataIndexInternal.LastInternalSlot + 6,
		Index7 = GladNetPayloadDataIndexInternal.LastInternalSlot + 7,
		Index8 = GladNetPayloadDataIndexInternal.LastInternalSlot + 8,
		Index9 = GladNetPayloadDataIndexInternal.LastInternalSlot + 9,
		Index10 = GladNetPayloadDataIndexInternal.LastInternalSlot + 10,
		Index11 = GladNetPayloadDataIndexInternal.LastInternalSlot + 11,
		Index12 = GladNetPayloadDataIndexInternal.LastInternalSlot + 12,
		Index13 = GladNetPayloadDataIndexInternal.LastInternalSlot + 13,
		Index14 = GladNetPayloadDataIndexInternal.LastInternalSlot + 14,
		Index15 = GladNetPayloadDataIndexInternal.LastInternalSlot + 15,
		Index16 = GladNetPayloadDataIndexInternal.LastInternalSlot + 16,
		Index17 = GladNetPayloadDataIndexInternal.LastInternalSlot + 17,
		Index18 = GladNetPayloadDataIndexInternal.LastInternalSlot + 18,
		Index19 = GladNetPayloadDataIndexInternal.LastInternalSlot + 19,
		Index20 = GladNetPayloadDataIndexInternal.LastInternalSlot + 20,
		Index21 = GladNetPayloadDataIndexInternal.LastInternalSlot + 21,
		Index22 = GladNetPayloadDataIndexInternal.LastInternalSlot + 22,
		Index23 = GladNetPayloadDataIndexInternal.LastInternalSlot + 23,
		Index24 = GladNetPayloadDataIndexInternal.LastInternalSlot + 24,
		Index25 = GladNetPayloadDataIndexInternal.LastInternalSlot + 25,
		Index26 = GladNetPayloadDataIndexInternal.LastInternalSlot + 26,
		Index27 = GladNetPayloadDataIndexInternal.LastInternalSlot + 27,
		Index28 = GladNetPayloadDataIndexInternal.LastInternalSlot + 28,
		Index29 = GladNetPayloadDataIndexInternal.LastInternalSlot + 29,
		Index30 = GladNetPayloadDataIndexInternal.LastInternalSlot + 30,
		Index31 = GladNetPayloadDataIndexInternal.LastInternalSlot + 31,
		Index32 = GladNetPayloadDataIndexInternal.LastInternalSlot + 32,
		Index33 = GladNetPayloadDataIndexInternal.LastInternalSlot + 33,
		Index34 = GladNetPayloadDataIndexInternal.LastInternalSlot + 34,
		Index35 = GladNetPayloadDataIndexInternal.LastInternalSlot + 35,
		Index36 = GladNetPayloadDataIndexInternal.LastInternalSlot + 36,
		Index37 = GladNetPayloadDataIndexInternal.LastInternalSlot + 37,
		Index38 = GladNetPayloadDataIndexInternal.LastInternalSlot + 38,
	}

	/// <summary>
	/// For GladNet2 internal use only. Uses to internally mark members of a Type as
	/// serializable members.
	/// </summary>
	internal enum GladNetPayloadDataIndexInternal : int
	{
		/// <summary>
		/// This Enum field indicates the last internal reserved value for Payload data indicies.
		/// </summary>
		LastInternalSlot = 1,
	}
}
