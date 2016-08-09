using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Payload
{
	/// <summary>
	/// Metadata to indicate that authorization is needed for the marked payload.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class AuthorizationRequiredAttribute : Attribute
	{

	}
}
