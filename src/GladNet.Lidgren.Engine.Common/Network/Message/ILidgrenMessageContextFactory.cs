using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Lidgren.Engine.Common
{
	/// <summary>
	/// Factory that generates <see cref="LidgrenMessageContext"/>s.
	/// </summary>
	public interface ILidgrenMessageContextFactory
	{
		/// <summary>
		/// Creates a <see cref="LidgrenMessageContext"/> based on the incoming
		/// <see cref="NetIncomingMessage"/> instance.
		/// </summary>
		/// <param name="message"></param>
		/// <returns>A non-null reference to a derived <see cref="LidgrenMessageContext"/>.</returns>
		LidgrenMessageContext CreateContext(NetIncomingMessage message);
	}
}
