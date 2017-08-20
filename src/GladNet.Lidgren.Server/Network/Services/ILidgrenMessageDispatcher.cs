using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GladNet.Lidgren.Engine.Common;

namespace GladNet.Lidgren.Server
{
	public interface ILidgrenMessageDispatcher
	{
		void DispatchMessages(IEnumerable<LidgrenMessageContext> messages);
	}
}
