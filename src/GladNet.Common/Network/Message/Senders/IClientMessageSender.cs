using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	public interface IClientMessageSender : INetworkMessageSender
	{
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		NetworkMessage.SendResult SendRequest<TRequestPacket>(TRequestPacket payload, NetworkMessage.DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0)
			where TRequestPacket : PacketPayload, IRequestPayload;
	}
}
