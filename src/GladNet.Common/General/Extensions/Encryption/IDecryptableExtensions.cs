using GladNet.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public static class IDecryptableExtensions
	{
		public static IDeserializable DecryptWith<TObjectType>(this TObjectType obj, IDecryptorStrategy decryptor)
			where TObjectType : IDecryptable, IDeserializable
		{
			obj.Decrypt(decryptor);

			return obj;
		}
	}
}
