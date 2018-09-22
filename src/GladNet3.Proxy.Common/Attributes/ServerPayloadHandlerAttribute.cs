using System;
using System.Collections.Generic;
using System.Text;

namespace GladNet
{
	/// <summary>
	/// Attribute that hints that the handler is a server payload handler.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ServerPayloadHandlerAttribute : Attribute
	{
		
	}
}
