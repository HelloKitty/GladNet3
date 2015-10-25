using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Serializer
{
	//This Type name clashes with an already established Type but it is the most fitting name https://msdn.microsoft.com/en-us/library/System.Runtime.Serialization.ISerializable(v=VS.110).aspx
	public interface ISerializable
	{
		bool Serialize(ISerializer serializer);
	}
}
