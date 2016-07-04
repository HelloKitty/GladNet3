using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Engine.Common
{
	public interface INetSender
	{
		/// <summary>
		/// Indicates if the <see cref="OperationType"/> can be sent with this peer.
		/// </summary>
		/// <param name="opType"><see cref="OperationType"/> to check.</param>
		/// <returns>True if the peer can see the <paramref name="opType"/>.</returns>
		bool CanSend(OperationType opType);
	}
}
