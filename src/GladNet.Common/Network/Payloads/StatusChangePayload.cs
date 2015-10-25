using GladNet.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	/// <summary>
	/// A wire-type payload that contains information about a <see cref="NetStauts"/> change.
	/// </summary>
	[GladNetSerializationContract]
	public class StatusChangePayload : PacketPayload
	{
		/// <summary>
		/// Indicates the <see cref="NetStats"/> of the change.
		/// </summary>
		[GladNetMember(1, IsRequired = true)]
		public NetStatus Status { get; private set; }

		/// <summary>
		/// Generates a <see cref="StatusChangePayload"/> instance with the given <see cref="NetStatus"/>
		/// </summary>
		/// <param name="status">NetStatus of the change.</param>
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
