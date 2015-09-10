using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public class StatusChangePayload : PacketPayload
	{
		public NetStatus Status { get; private set; }

		public StatusChangePayload(NetStatus status)
		{
			Status = status;
		}

		protected StatusChangePayload()
		{

		}
	}
}
