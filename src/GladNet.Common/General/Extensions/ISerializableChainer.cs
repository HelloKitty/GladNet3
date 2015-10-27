using GladNet.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public interface ISerializableChainer<TChainingType>
		where TChainingType : ISerializable
	{
		TChainingType SerializeWith(ISerializer serializer);

		//TChainingType SerializeWith(Func<ISerializer> serializerFunc);
	}
}
