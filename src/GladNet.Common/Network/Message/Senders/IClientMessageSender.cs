using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	public interface IClientMessageSender : INetworkMessageSender
	{
		NetworkMessage.SendResult SendRequest<RequestPacketType>(RequestPacketType payload, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
			where RequestPacketType : PacketPayload, PacketPayload.IRequest;
	}
}
