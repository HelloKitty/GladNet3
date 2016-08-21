using GladNet.Lidgren.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using GladNet.Message;

namespace GladNet.Lidgren.Engine.Common
{
	/// <summary>
	/// Lidgren message context for <see cref="IStatusMessage"/>s.
	/// </summary>
	public class LidgrenStatusChangeMessageContext : LidgrenMessageContext
	{
		/// <summary>
		/// The generated message.
		/// </summary>
		public IStatusMessage GeneratedStatusMessage { get; }

		public LidgrenStatusChangeMessageContext(NetIncomingMessage incomingMessage) 
			: base(incomingMessage)
		{
			//Generate a Lidgren status message for dispatching
			GeneratedStatusMessage = new LidgrenStatusMessage((NetConnectionStatus)incomingMessage.ReadByte());
		}

		public override bool TryDispatch(INetworkMessageReceiver reciever)
		{
			if (GeneratedStatusMessage == null)
				return false;

			reciever.OnNetworkMessageReceive(GeneratedStatusMessage, this); //pass to the reciever the message and this (message parameters)

			return true;
		}
	}
}
