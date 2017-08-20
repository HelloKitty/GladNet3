using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Serializer.Protobuf
{
	/// <summary>
	/// Serializable surrogate type for the Stack{int} type.
	/// </summary>
	[ProtoContract]
	public class StackIntSurrogate
	{
		//string is serializable so we'll just copy this property back and forth
		[ProtoMember(1)]
		public List<int> surrogatedStackList { get; private set; } = null;

		public StackIntSurrogate()
		{

		}

		public static implicit operator Stack<int>(StackIntSurrogate surrogate)
		{
			//handles null surrogate AND null surrogate list
			return surrogate == null ? null : ((surrogate == null || surrogate.surrogatedStackList == null) ? null : new Stack<int>(surrogate.surrogatedStackList.Reverse<int>()));
		}

		public static implicit operator StackIntSurrogate(Stack<int> source)
		{
			return source == null ? null : new StackIntSurrogate
			{
				surrogatedStackList = source?.ToList()
			};
		}
	}
}
