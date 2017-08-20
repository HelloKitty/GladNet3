using GladNet.Lidgren.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GladNet.Engine.Common;

namespace GladNet.Lidgren.Client
{
	public class ClientSendServiceSelectionStrategy : ISendServiceSelectionStrategy
	{
		private INetworkMessagePayloadSenderService ClientMessageSender { get; }

		public ClientSendServiceSelectionStrategy(INetworkMessagePayloadSenderService clientMessageSender)
		{
			if (clientMessageSender == null) throw new ArgumentNullException(nameof(clientMessageSender));

			ClientMessageSender = clientMessageSender;
		}

		/// <inheritdoc />
		public INetworkMessagePayloadSenderService GetSendingService(int connectionId)
		{
			//Doesn't matter what the connection id is. A client only has one send service
			return ClientMessageSender;
		}
	}
}
