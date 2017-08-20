using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GladNet.Lidgren.Engine.Common;

namespace GladNet.Lidgren.Server
{
	public interface ILidgrenMessagePollable
	{
		/// <summary>
		/// Polls for any available messages.
		/// </summary>
		/// <returns>The messages or an empty array of messages.</returns>
		LidgrenMessageContext[] Poll();
	}
}
