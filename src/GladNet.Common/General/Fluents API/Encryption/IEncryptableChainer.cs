using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public interface IEncryptableChainer<TChainingType>
		where TChainingType : IEncryptable
	{
		TChainingType EncryptWith(IEncryptor encryptor);

		//TChainingType EncryptWith(Func<IEncryptor> encryptor);
	}
}
