using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Serializer
{
	//The reason this was added is because we MUST reserve indicies for future use
	//in GladNet2 internals but at the same time we must let users specify the index at compile time
	//in an attribute. To do this we require users to provide a GladNetPayloadIncludeIndex
	public enum GladNetPayloadIncludeIndex
	{
		//TODO: Create more of these... Lots more.
		Index1 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 1,
		Index2 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 2,
		Index3 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 3,
		Index4 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 4,
		Index5 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 5,
		Index6 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 6,
		Index7 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 7,
		Index8 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 8,
		Index9 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 9,
		Index10 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 10,
		Index11 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 11,
		Index12 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 12,
		Index13 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 13,
		Index14 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 14,
		Index15 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 15,
		Index16 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 16,
		Index17 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 17,
		Index18 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 18,
		Index19 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 19,
		Index20 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 20,
		Index21 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 21,
		Index22 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 22,
		Index23 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 23,
		Index24 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 24,
		Index25 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 25,
		Index26 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 26,
		Index27 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 27,
		Index28 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 28,
		Index29 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 29,
		Index30 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 30,
		Index31 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 31,
		Index32 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 32,
		Index33 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 33,
		Index34 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 34,
		Index35 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 35,
		Index36 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 36,
		Index37 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 37,
		Index38 = GladNetPayloadIncludeInternalIndex.LastInternalSlot + 38,
	}

	internal enum GladNetPayloadIncludeInternalIndex : int
	{
		//We don't currently reserve any payload include values
		/// <summary>
		/// This Enum field indicates the last internal reserved value for Payload data indicies.
		/// </summary>
		LastInternalSlot = 0,
	}
}
