using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	public class ResponseMessage : NetworkMessage, IResponseMessage
	{
		public ResponseMessage(PacketPayload payload)
			: base(payload)
		{

		}


		public override void Dispatch(INetworkMessageReceiver receiver, IMessageParameters mParams)
		{
			receiver.OnNetworkMessageRecieve(this, mParams);
		}
	}
}
