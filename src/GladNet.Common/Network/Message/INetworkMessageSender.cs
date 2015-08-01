using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	public interface INetworkMessageSender
	{
		NetworkMessage.SendResult SendRequest(PacketPayload payload, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0);

		NetworkMessage.SendResult SendResponse(PacketPayload payload, NetworkMessage.DeliveryMethod deliveryMethod, byte responseCode, bool encrypt = false, byte channel = 0);

		NetworkMessage.SendResult SendEvent(PacketPayload payload, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0);
	}
}
