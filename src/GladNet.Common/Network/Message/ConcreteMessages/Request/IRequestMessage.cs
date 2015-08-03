using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GladNet.Common
{
	/// <summary>
	/// <see cref="IRequestMessage"/>s are <see cref="INetworkMessage"/>s that request remote peers for/to-do something.
	/// Generally these ellict <see cref="IResponseMessage"/> but there is no implict mechanism in either <see cref="INetworkMessage"/>
	/// Subtypes for such a thing.
	/// </summary>
	public interface IRequestMessage : INetworkMessage
	{

	}
}
