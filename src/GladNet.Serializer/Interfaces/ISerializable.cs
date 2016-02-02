using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Serializer
{
	//This Type name clashes with an already established Type but it is the most fitting name https://msdn.microsoft.com/en-us/library/System.Runtime.Serialization.ISerializable(v=VS.110).aspx
	/// <summary>
	/// Implementer is an object that can be serialized
	/// </summary>
	public interface ISerializable
	{
		/// <summary>
		/// Attempts to serialize an object with a given serialization strategy.
		/// </summary>
		/// <param name="serializer">Serialization strategy.</param>
		/// <returns>Indicates if the serialization was successful.</returns>
		bool Serialize(ISerializer serializer);
	}
}
