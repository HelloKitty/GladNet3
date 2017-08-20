using GladNet.Common;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Lidgren.Common
{
	public static class NetConnectionDetailsExtensions
	{
		public static NetStatus ToGladNet(this NetConnectionStatus status)
		{
			switch (status)
			{
				case NetConnectionStatus.InitiatedConnect:
					return NetStatus.Connecting;

				case NetConnectionStatus.ReceivedInitiation:
					return NetStatus.Connecting;

				case NetConnectionStatus.RespondedAwaitingApproval:
					return NetStatus.Connecting;

				case NetConnectionStatus.RespondedConnect:
					return NetStatus.Connecting;

				case NetConnectionStatus.Connected:
					return NetStatus.Connected;

				case NetConnectionStatus.Disconnecting:
					return NetStatus.Disconnecting;

				case NetConnectionStatus.Disconnected:
					return NetStatus.Disconnected;

				default:
					throw new InvalidOperationException($"Tried to map {nameof(NetStatus)} to Lidgren {nameof(NetConnectionStatus)} but no valid mapping was available for value: {status}");
			}
		}
	}
}
