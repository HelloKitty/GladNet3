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

		public bool AllowReceiverEmulation { get; set; }

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
		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		void INetworkMessageReceiver.OnNetworkMessageReceive(IRequestMessage message, IMessageParameters parameters)
		{
			OnReceiveRequest(message, parameters);
		}

		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		void INetworkMessageReceiver.OnNetworkMessageReceive(IResponseMessage message, IMessageParameters parameters)
		{
			OnReceiveResponse(message, parameters);
		}

		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		void INetworkMessageReceiver.OnNetworkMessageReceive(IEventMessage message, IMessageParameters parameters)
		{
			OnReceiveEvent(message, parameters);
		}

		[SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
		void INetworkMessageReceiver.OnStatusChanged(NetStatus status)
		{
			OnStatusChanged(status);
		}

		//To stay in line with MS recommendation on explict implementions of interfaces we MUST provide a public callable replacement for INetworkMessageReciever.
		//Refer to this: https://msdn.microsoft.com/en-us/library/ms182153.aspx for more information.
		public void EmulateOnNetworkMessageReceive(IRequestMessage message, IMessageParameters parameters)
		{
			if (!AllowReceiverEmulation)
				throw new InvalidOperationException("Unable to emulate network receive method. Emulation must be explicitly enabled.");

			OnReceiveRequest(message, parameters);
		}

		public void EmulateOnNetworkMessageReceive(IResponseMessage message, IMessageParameters parameters)
		{
			if (!AllowReceiverEmulation)
				throw new InvalidOperationException("Unable to emulate network receive method. Emulation must be explicitly enabled.");

			OnReceiveResponse(message, parameters);
		}

		public void EmulateOnNetworkMessageReceive(IEventMessage message, IMessageParameters parameters)
		{
			if (!AllowReceiverEmulation)
				throw new InvalidOperationException("Unable to emulate network receive method. Emulation must be explicitly enabled.");

			OnReceiveEvent(message, parameters);
		}

		public void EmulateOnStatusChanged(NetStatus status)
		{
			if (!AllowReceiverEmulation)
				throw new InvalidOperationException("Unable to emulate network receive method. Emulation must be explicitly enabled.");

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
