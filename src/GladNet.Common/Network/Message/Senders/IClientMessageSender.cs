using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	public interface IClientMessageSender
	{
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		NetworkMessage.SendResult SendResponse(PacketPayload payload, NetworkMessage.DeliveryMethod deliveryMethod, byte responseCode, bool encrypt = false, byte channel = 0);

		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		NetworkMessage.SendResult SendEvent(PacketPayload payload, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0);
	}
}
