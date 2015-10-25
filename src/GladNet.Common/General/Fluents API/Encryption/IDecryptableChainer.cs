using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public interface IDecryptableChainer<TChainingType>
		where TChainingType : IDecryptable
	{
		TChainingType EncryptWith(IDecryptor decryptor);

		//TChainingType EncryptWith(Func<IDecryptor> decryptorFunc);
	}
}
