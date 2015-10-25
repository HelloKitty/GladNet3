using Logging.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;


namespace GladNet.Common
{
	public abstract class Peer : INetPeer, INetworkMessageReceiver, IClassLogger
	{
		private readonly INetworkMessageSender netMessageSender;

		/// <summary>
		/// Provides access to various connection related details for a this given Pee's connection.
		/// </summary>
		public IConnectionDetails PeerDetails { get; private set; }

		/// <summary>
		/// Enables or disables emulation methods for recieving.
		/// </summary>
		public bool AllowReceiverEmulation { get; set; }

		public ILogger Logger { get; private set; }

		protected Peer(ILogger logger, INetworkMessageSender messageSender, IConnectionDetails details)
		{
			PeerDetails = details;
			netMessageSender = messageSender;
			Logger = logger;
		}

		#region Message Senders
		//The idea here is we return invalids because sometimes a Peer can't send a certain message type.
		//In most cases external classes shouldn't be interfacing with this class in this fashion.
		//They should instead used more explict send methods. However, this exists to allow for
		//users to loosely couple their senders as best they can though they really shouldn't since
		//it can't be known if the runetime Peer type offers that functionality.
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public virtual SendResult TrySendMessage(OperationType opType, PacketPayload payload, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
		{
			if (payload == null)
				throw new ArgumentNullException("payload", "Payload found null in: " + this.GetType() + " in TrySendMessage no params");

			//TODO: Implement logging.
			if (!CanSend(opType))
				return SendResult.Invalid;

			return netMessageSender.TrySendMessage(opType, payload, deliveryMethod, encrypt, channel); //ncrunch: no coverage Reason: The line doesn't have to be tested. This is abstract and can be overidden.
		}

		//This is non-virtual because it should mirror non-generic methods functionality. It makes no sense to change them individually.
		public SendResult TrySendMessage<TPacketType>(OperationType opType, TPacketType payload) where TPacketType : PacketPayload, IStaticPayloadParameters
		{
			if (payload == null)
				throw new ArgumentNullException("payload", "Payload found null in: " + this.GetType() + " in TrySendMessage no params");

			return TrySendMessage(opType, payload, payload.DeliveryMethod, payload.Encrypted, payload.Channel);
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

		public virtual bool CanSend(OperationType opType)
		{
			return false;
		}
	}
}
