using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	public interface IEncryptable
	{
		bool Encrypt(IEncryptor encryptor);

		bool Decrypt(IDecryptor decryptor);
	}
}
