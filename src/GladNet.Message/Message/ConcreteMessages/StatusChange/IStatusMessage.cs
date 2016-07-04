using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Message
{
	public interface IStatusMessage : INetworkMessage
	{
		NetStatus Status { get; }
	}
}
