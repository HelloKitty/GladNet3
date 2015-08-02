using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	public interface INetworkMessageSender
	{
		NetworkMessage.SendResult SendMessage(PacketPayload payload, PacketPayload.IRequest requestParameters, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0);

		NetworkMessage.SendResult SendMessage(PacketPayload payload, PacketPayload.IEvent eventParameters, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0);

		NetworkMessage.SendResult SendMessage(PacketPayload payload, PacketPayload.IResponse responseParameters, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0);
	}
}
