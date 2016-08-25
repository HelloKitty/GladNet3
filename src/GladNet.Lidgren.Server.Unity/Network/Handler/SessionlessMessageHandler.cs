using Common.Logging;
using GladNet.Engine.Common;
using GladNet.Lidgren.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Lidgren.Server.Unity
{
	public class SessionlessMessageHandler : IClassLogger
	{
		private IClientSessionFactory sessionFactory { get; }

		public ILog Logger { get; }

		public SessionlessMessageHandler(IClientSessionFactory factory, ILog logger)
		{
			if (factory == null)
				throw new ArgumentNullException(nameof(factory), $"Provided {nameof(IClientSessionFactory)} cannot be null.");

			Logger = logger;
			sessionFactory = factory;
		}

		public void HandleMessage(LidgrenMessageContext messageContext)
		{
			Logger.Debug($"Recieved unconnected message of Type: {messageContext.GetType().Name} ConnectionId: {messageContext.ConnectionId}.");

			//Messages with no connection ID assigned aren't handlable
			//IDs are only assigned when connection has been fully established
			if (messageContext.ConnectionId == 0)
				return;

			LidgrenStatusChangeMessageContext message = messageContext as LidgrenStatusChangeMessageContext;

			if (message == null)
				return;

			//We only care about status messages
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
			sessionFactory.Create(new LidgrenConnectionDetailsAdapter(message.IncomingMessage.SenderConnection.RemoteEndPoint.Address.ToString(), message.IncomingMessage.SenderConnection.RemoteEndPoint.Port, 0, message.ConnectionId), message.IncomingMessage.SenderConnection);
		}
	}
}
