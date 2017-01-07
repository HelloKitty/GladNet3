using GladNet.Common;
using GladNet.Message;
using GladNet.Payload;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;


namespace GladNet.Engine.Server
{
	/// <summary>
	/// Contracts implementing types to offer specific/supported network message types such as; Response and Event.
	/// </summary>
	public interface IClientSessionNetworkMessageSender : IResponsePayloadSender, IEventPayloadSender
	{

	}
}
