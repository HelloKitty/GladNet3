using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Serializer
{
	/// <summary>
	/// Implementer can register <see cref="Type"/>s for serialization.
	/// </summary>
	public interface ISerializerRegistry
	{
		bool Register(Type typeToRegister);
	}
}
