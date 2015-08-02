using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	/// <summary>
	/// Represents the GladNet smallest unit of transmittable networked information.
	/// </summary>
	public abstract class PacketPayload
	{
		public interface IResponse
		{
			byte ResponseCode { get; }
		}

		public interface IRequest
		{
			//To be filled out in the future.
		}

		public interface IEvent
		{
			//To be filled out in the future.
		}
	}
}
