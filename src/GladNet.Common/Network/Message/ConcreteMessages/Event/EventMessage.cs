using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	public class EventMessage : NetworkMessage, IEventMessage
	{

		public EventMessage(PacketPayload payload)
			: base(payload)
		{

		}

		public override void Dispatch(INetworkMessageReceiver receiver, IMessageParameters mParams)
		{
			receiver.OnNetworkMessageRecieve(this, mParams);
		}
	}
}
