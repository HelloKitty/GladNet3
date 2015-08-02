using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	public interface IServerMessageSender : INetworkMessageSender
	{
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		NetworkMessage.SendResult SendResponse<ResponsePacketType>(ResponsePacketType payload, NetworkMessage.DeliveryMethod deliveryMethod, byte responseCode, bool encrypt = false, byte channel = 0)
			where ResponsePacketType : PacketPayload, PacketPayload.IResponse;

		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		NetworkMessage.SendResult SendEvent<EventPacketType>(EventPacketType payload, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
			where EventPacketType : PacketPayload, PacketPayload.IEvent;
	}
}
