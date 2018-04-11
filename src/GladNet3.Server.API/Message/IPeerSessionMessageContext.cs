using System;
using System.Collections.Generic;
using System.Text;

namespace GladNet
{
	/// <summary>
	/// Contract for session/server peer connections message contexts.
	/// </summary>
	/// <typeparam name="TPayloadType">Outgoing payload type.</typeparam>
	public interface IPeerSessionMessageContext<in TPayloadType> : IPeerMessageContext<TPayloadType> 
		where TPayloadType : class
	{
		/// <summary>
		/// The deatils of the session.
		/// </summary>
		SessionDetails Details { get; }
	}
}
