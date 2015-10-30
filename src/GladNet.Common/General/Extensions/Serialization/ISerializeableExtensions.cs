using GladNet.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public static class ISerializeableExtensions
	{
		public static IEncryptable SerializeWith<TObjectType>(this TObjectType obj, ISerializer serializer)
			where TObjectType : ISerializable, IEncryptable
		{
			obj.Serialize(serializer);

			return obj;
		}
	}
}
