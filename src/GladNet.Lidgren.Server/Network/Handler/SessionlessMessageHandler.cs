using Common.Logging;
using GladNet.Engine.Common;
using GladNet.Lidgren.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Lidgren.Server
{
	public class SessionlessMessageHandler : IClassLogger
	{
		/// <summary>
		/// The factory that creates client sessions.
		/// </summary>
		private IClientSessionFactory SessionFactory { get; }

		/// <inheritdoc />
		public ILog Logger { get; }

		public SessionlessMessageHandler(IClientSessionFactory factory, ILog logger)
		{
			if (factory == null) throw new ArgumentNullException(nameof(factory));
			if (logger == null) throw new ArgumentNullException(nameof(logger));

			Logger = logger;
			SessionFactory = factory;
		}

		public void HandleMessage(LidgrenMessageContext messageContext)
		{
			if (messageContext == null) throw new ArgumentNullException(nameof(messageContext));

			if (Logger.IsDebugEnabled)
				Logger.Debug($"Recieved unconnected message of Type: {messageContext.GetType().Name} ConnectionId: {messageContext.ConnectionId}.");

			//Messages with no connection ID assigned aren't handlable
			//IDs are only assigned when connection has been fully established
			if (messageContext.ConnectionId == 0)
				return;

			LidgrenStatusChangeMessageContext message = messageContext as LidgrenStatusChangeMessageContext;

			if (message == null)
				return;

			//We only care about status messages
			if(Logger.IsDebugEnabled)
				Logger.Debug($"Recieved StatusChange: {message.GeneratedStatusMessage.Status.ToString()} LidgrenStatus: {message.LidgrenStatus.ToString()}");

			switch (message.GeneratedStatusMessage.Status)
			{
				//Return if not about connected satus change
				case GladNet.Common.NetStatus.Connected:
					break;
				default:
					return;
			}

			//create the connection details and then create the peer
			//We don't really need to do anything with the session created
			SessionFactory.Create(new LidgrenConnectionDetailsAdapter(message.IncomingMessage.SenderConnection.RemoteEndPoint.Address.ToString(), message.IncomingMessage.SenderConnection.RemoteEndPoint.Port, 0, message.ConnectionId), message.IncomingMessage.SenderConnection);
		}
	}
}
