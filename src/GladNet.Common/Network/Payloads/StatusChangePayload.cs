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

		//Protobuf-net constructors should be marked with ncrunch no coverage to suppress it from coverage metrics
		//ncrunch: no coverage start
		/// <summary>
		/// Empty protobuf-net constuctor
		/// </summary>
		protected StatusChangePayload()
		{

		}
		//ncrunch: no coverage end
	}
}
