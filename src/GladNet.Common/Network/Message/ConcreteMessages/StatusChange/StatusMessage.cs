using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public class StatusMessage : NetworkMessage, IStatusMessage
	{

		public NetStatus? Status
		{
			get
			{
				//This isn't great but we just check the Type and if it's valid we as cast.
				//The original design didn't anticipate NetStatus messages which could have been just a byte instead of a whole PacketPayload
				if (Payload.Data is StatusChangePayload)
					return (Payload.Data as StatusChangePayload).Status;
				else
					return null;
			}
		}

		public StatusMessage(StatusChangePayload payload)
			: base(payload)
		{
			//This ensures a non-malicious sender sends a StatusChangePayload but it must still be checked.
		}

		public override void Dispatch(INetworkMessageReceiver receiver, IMessageParameters parameters = null)
		{
			//We don't need IMessageParameters for this type of message.

			if (Status.HasValue)
				receiver.OnStatusChanged(Status.Value);
#if DEBUG || DEBUGBUILD
			else
				throw new Exception("Recieved status message with non-StatusChangePayload payload Type.");
#endif
		}
	}
}
