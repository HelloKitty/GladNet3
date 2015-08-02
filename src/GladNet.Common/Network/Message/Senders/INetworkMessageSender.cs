using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	public interface INetworkMessageSender
	{
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		NetworkMessage.SendResult SendMessage(PacketPayload payload, IRequestPayload requestParameters, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0);

		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		NetworkMessage.SendResult SendMessage(PacketPayload payload, IEventPayload eventParameters, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0);

		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		NetworkMessage.SendResult SendMessage(PacketPayload payload, IResponsePayload responseParameters, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0);
	}
}
