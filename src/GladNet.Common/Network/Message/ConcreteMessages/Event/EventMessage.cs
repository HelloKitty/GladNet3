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
#if DEBUG || DEBUGBUILD
			if(receiver == null)
				throw new ArgumentNullException("receiver", typeof(INetworkMessageReceiver).ToString() + " parameter is null in " + GetType().ToString());

			if(mParams == null)
				throw new ArgumentNullException("mParams", typeof(IMessageParameters).ToString() + " parameter is null in " + GetType().ToString());
#endif

			receiver.OnNetworkMessageRecieve(this, mParams);
		}
	}
}
