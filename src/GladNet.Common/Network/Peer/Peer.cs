using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	public abstract class Peer : INetPeer, INetworkMessageReceiver
	{
		protected readonly INetEngine NetEngine;

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
		public virtual NetworkMessage.SendResult TrySendMessage(PacketPayload payload, IRequestPayload requestParameters, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			if (payload == null)
				throw new ArgumentException("Payload found null in: " + this.GetType() + " in TrySendMessage for params " + requestParameters.GetType(), "payload");

			//TODO: Implement logging.
			if (!CanSend(NetworkMessage.OperationType.Request))
				return NetworkMessage.SendResult.Invalid;

			if (requestParameters == null)
				TrySendMessage(NetworkMessage.OperationType.Request, payload, deliveryMethod, encrypt, channel);

			//TODO: Implement sending
			throw new NotImplementedException();
		}

		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public virtual NetworkMessage.SendResult TrySendMessage(PacketPayload payload, IEventPayload eventParameters, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			if (payload == null)
				throw new ArgumentException("Payload found null in: " + this.GetType() + " in TrySendMessage for params " + eventParameters.GetType(), "payload");

			//TODO: Implement logging.
			if (!CanSend(NetworkMessage.OperationType.Event))
				return NetworkMessage.SendResult.Invalid;

			if (eventParameters == null)
				TrySendMessage(NetworkMessage.OperationType.Event, payload, deliveryMethod, encrypt, channel);

			//TODO: Implement sending
			throw new NotImplementedException();
		}

		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public virtual NetworkMessage.SendResult TrySendMessage(PacketPayload payload, IResponsePayload responseParameters, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			if (payload == null)
				throw new ArgumentException("Payload found null in: " + this.GetType() + " in TrySendMessage for params " + responseParameters.GetType(), "payload");

			//TODO: Implement logging.
			if (!CanSend(NetworkMessage.OperationType.Response))
				return NetworkMessage.SendResult.Invalid;

			if (responseParameters == null)
				TrySendMessage(NetworkMessage.OperationType.Response, payload, deliveryMethod, encrypt, channel);

			//TODO: Implement sending
			throw new NotImplementedException();
		}

		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public virtual NetworkMessage.SendResult TrySendMessage(NetworkMessage.OperationType opType, PacketPayload payload, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			if (payload == null)
				throw new ArgumentException("Payload found null in: " + this.GetType() + " in TrySendMessage no params", "payload");

			//TODO: Implement logging.
			if (!CanSend(opType))
				return NetworkMessage.SendResult.Invalid;

			//TODO: Implement sending
			throw new NotImplementedException();
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

		protected abstract void OnReceiveRequest(IRequestMessage message, IMessageParameters parameters);
		protected abstract void OnReceiveResponse(IResponseMessage message, IMessageParameters parameters);
		protected abstract void OnReceiveEvent(IEventMessage message, IMessageParameters parameters);
		#endregion

		public virtual bool CanSend(NetworkMessage.OperationType opType)
		{
			return false;
		}
	}
}
