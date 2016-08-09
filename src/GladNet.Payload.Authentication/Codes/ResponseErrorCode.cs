using System;
using System.Collections.Generic;
using System.Linq;

namespace GladNet.Payload.Authentication
{
	/// <summary>
	/// oAuth2 strongly-typed error response codes.
	/// </summary>
	public enum ResponseErrorCode : byte
	{
		//TODO: Implement tha oAuth error code spec on the specification page 27-28
		//For now a simple Error will work
		Error = 0
	}
}
