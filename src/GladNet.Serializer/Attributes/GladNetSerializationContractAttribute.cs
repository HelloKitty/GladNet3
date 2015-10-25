using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Serializer
{
	//This attribute usage is closely based on the requirements put forth in https://github.com/mgravell/protobuf-net/blob/e601b359c6ae56afc159754d29f5e7d0f05a01f5/protobuf-net/ProtoContractAttribute.cs
	//They MAY change but GladNet is loosely based on Protobuf-net for serialization so the abstraction is based on the requirements of Protobuf-net.
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface, 
		AllowMultiple = false, Inherited = false)]
	public sealed class GladNetSerializationContractAttribute : Attribute
	{
		//This class will likely stay mostly empty and will only have
		//optional arguements and members added.
		public GladNetSerializationContractAttribute()
			: base()
		{

		}
	}
}
