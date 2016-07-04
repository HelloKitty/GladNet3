using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Serializer
{
	//The reason this was added is because we MUST reserve indicies for future use
	//in GladNet2 internals but at the same time we must let users specify the index at compile time
	//in an attribute. To do this we require users to provide a GladNetIncludeIndex
	public enum GladNetIncludeIndex
	{
		//TODO: Create more of these... Lots more.
		Index1 = GladNetIncludeInternalIndex.LastInternalSlot + 1,
		Index2 = GladNetIncludeInternalIndex.LastInternalSlot + 2,
		Index3 = GladNetIncludeInternalIndex.LastInternalSlot + 3,
		Index4 = GladNetIncludeInternalIndex.LastInternalSlot + 4,
		Index5 = GladNetIncludeInternalIndex.LastInternalSlot + 5,
		Index6 = GladNetIncludeInternalIndex.LastInternalSlot + 6,
		Index7 = GladNetIncludeInternalIndex.LastInternalSlot + 7,
		Index8 = GladNetIncludeInternalIndex.LastInternalSlot + 8,
		Index9 = GladNetIncludeInternalIndex.LastInternalSlot + 9,
		Index10 = GladNetIncludeInternalIndex.LastInternalSlot + 10,
		Index11 = GladNetIncludeInternalIndex.LastInternalSlot + 11,
		Index12 = GladNetIncludeInternalIndex.LastInternalSlot + 12,
		Index13 = GladNetIncludeInternalIndex.LastInternalSlot + 13,
		Index14 = GladNetIncludeInternalIndex.LastInternalSlot + 14,
		Index15 = GladNetIncludeInternalIndex.LastInternalSlot + 15,
		Index16 = GladNetIncludeInternalIndex.LastInternalSlot + 16,
		Index17 = GladNetIncludeInternalIndex.LastInternalSlot + 17,
		Index18 = GladNetIncludeInternalIndex.LastInternalSlot + 18,
		Index19 = GladNetIncludeInternalIndex.LastInternalSlot + 19,
		Index20 = GladNetIncludeInternalIndex.LastInternalSlot + 20,
		Index21 = GladNetIncludeInternalIndex.LastInternalSlot + 21,
		Index22 = GladNetIncludeInternalIndex.LastInternalSlot + 22,
		Index23 = GladNetIncludeInternalIndex.LastInternalSlot + 23,
		Index24 = GladNetIncludeInternalIndex.LastInternalSlot + 24,
		Index25 = GladNetIncludeInternalIndex.LastInternalSlot + 25,
		Index26 = GladNetIncludeInternalIndex.LastInternalSlot + 26,
		Index27 = GladNetIncludeInternalIndex.LastInternalSlot + 27,
		Index28 = GladNetIncludeInternalIndex.LastInternalSlot + 28,
		Index29 = GladNetIncludeInternalIndex.LastInternalSlot + 29,
		Index30 = GladNetIncludeInternalIndex.LastInternalSlot + 30,
		Index31 = GladNetIncludeInternalIndex.LastInternalSlot + 31,
		Index32 = GladNetIncludeInternalIndex.LastInternalSlot + 32,
		Index33 = GladNetIncludeInternalIndex.LastInternalSlot + 33,
		Index34 = GladNetIncludeInternalIndex.LastInternalSlot + 34,
		Index35 = GladNetIncludeInternalIndex.LastInternalSlot + 35,
		Index36 = GladNetIncludeInternalIndex.LastInternalSlot + 36,
		Index37 = GladNetIncludeInternalIndex.LastInternalSlot + 37,
		Index38 = GladNetIncludeInternalIndex.LastInternalSlot + 38,
	}

	internal enum GladNetIncludeInternalIndex : int
	{
		//We don't currently reserve any payload include values
		/// <summary>
		/// This Enum field indicates the last internal reserved value for data indicies.
		/// </summary>
		LastInternalSlot = 0,
	}
}
