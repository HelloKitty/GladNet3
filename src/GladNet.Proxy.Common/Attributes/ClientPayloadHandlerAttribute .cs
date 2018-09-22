using System;
using System.Collections.Generic;
using System.Text;

namespace GladNet
{
	/// <summary>
	/// Attribute that hints that the handler is a client payload handler.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = true)]
	public sealed class ClientPayloadHandlerAttribute : Attribute
	{
		
	}
}
