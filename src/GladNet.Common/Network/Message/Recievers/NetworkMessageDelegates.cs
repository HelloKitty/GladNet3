using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public delegate void OnNetworkEventMessage(IEventMessage eventMessage, IMessageParameters parameters);
	public delegate void OnNetworkRequestMessage(IRequestMessage eventMessage, IMessageParameters parameters);
	public delegate void OnNetworkResponseMessage(IResponseMessage eventMessage, IMessageParameters parameters);
	public delegate void OnNetworkStatusMessage(IStatusMessage eventMessage, IMessageParameters parameters);
}
