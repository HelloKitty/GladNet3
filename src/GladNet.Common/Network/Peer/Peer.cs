using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;


namespace GladNet.Common
{
	public abstract class Peer : INetPeer, INetworkMessageReceiver
	{
		private readonly INetEngine NetEngine;

		public IConnectionDetails PeerDetails
		{
			get { return NetEngine.Details; }
		}

		protected Peer(INetEngine engine)
		{
			NetEngine = engine;
		}

		#region Message Senders
		//The idea here is we return invalids because sometimes a Peer can't send a certain message type.
		//In most cases external classes shouldn't be interfacing with this class in this fashion.
		//They should instead used more explict send methods. However, this exists to allow for
		//users to loosely couple their senders as best they can though they really shouldn't since
		//it can't be known if the runetime Peer type offers that functionality.
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public virtual NetworkMessage.SendResult TrySendMessage(NetworkMessage.OperationType opType, PacketPayload payload, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			if (payload == null)
				throw new ArgumentException("Payload found null in: " + this.GetType() + " in TrySendMessage no params", "payload");

			//TODO: Implement logging.
			if (!CanSend(opType))
				return NetworkMessage.SendResult.Invalid;

			return NetEngine.TrySendMessage(opType, payload, deliveryMethod, encrypt, channel);
		}
		#endregion

		#region Message Receivers
		void INetworkMessageReceiver.OnNetworkMessageReceive(IRequestMessage message, IMessageParameters parameters)
		{
			OnReceiveRequest(message, parameters);
		}

		void INetworkMessageReceiver.OnNetworkMessageReceive(IResponseMessage message, IMessageParameters parameters)
		{
			OnReceiveResponse(message, parameters);
		}

		void INetworkMessageReceiver.OnNetworkMessageReceive(IEventMessage message, IMessageParameters parameters)
		{
			OnReceiveEvent(message, parameters);
		}

		void INetworkMessageReceiver.OnStatusChanged(NetStatus status)
		{
			OnStatusChanged(status);
		}

		protected abstract void OnReceiveRequest(IRequestMessage message, IMessageParameters parameters);
		protected abstract void OnReceiveResponse(IResponseMessage message, IMessageParameters parameters);
		protected abstract void OnReceiveEvent(IEventMessage message, IMessageParameters parameters);
		protected abstract void OnStatusChanged(NetStatus status);
		#endregion

		public virtual bool CanSend(NetworkMessage.OperationType opType)
		{
			return false;
		}
	}
}
