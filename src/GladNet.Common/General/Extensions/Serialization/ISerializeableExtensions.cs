using GladNet.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public static class ISerializeableExtensions
	{
		/// <summary>
		/// Serializes an object with the provided deserialization strategy.
		/// </summary>
		/// <typeparam name="TObjectType">Type being serialized.</typeparam>
		/// <param name="obj">Object being serialized.</param>
		/// <param name="decryptor">Strategy for serialization.</param>
		/// <returns>A a serialized instance of the object that can be encrypted.</returns>
		public static IEncryptable SerializeWith<TObjectType>(this TObjectType obj, ISerializerStrategy serializer)
			where TObjectType : ISerializable, IEncryptable
		{
			//serialize the object and return the instance for fluent chaining
			obj.Serialize(serializer);

			return obj;
		}
	}
}
