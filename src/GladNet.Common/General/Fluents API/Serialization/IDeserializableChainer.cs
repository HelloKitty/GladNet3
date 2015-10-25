using GladNet.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public interface IDeserializableChainer<TChainingType>
		where TChainingType : IDeserializable
	{
		TChainingType DeserializeWith(IDeserializer deserializer);

		//TChainingType DeserializeWith(Func<IDeserializer> deserializerFunc);
	}
}
