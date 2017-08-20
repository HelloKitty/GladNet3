using GladNet.Common;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Lidgren.Common
{
	public static class NetSendResultExtensions
	{
		/// <summary>
		/// This translates <see cref="NetSendResult"/> to GladNet <see cref="SendResult"/>
		/// Information related to this translation can be found here https://code.google.com/p/lidgren-network-gen3/wiki/Basics
		/// </summary>
		/// <param name="result">The value to be used for translation.</param>
		/// <exception cref="ArgumentException">Throws an exception if the <see cref="NetSendResult"/> is undefined.</exception>
		/// <returns>The equivalent <see cref="SendResult"/> for the given <see cref="NetSendResult"/> value.</returns>
		public static SendResult ToGladNet(this NetSendResult result)
		{
			switch (result)
			{
				case NetSendResult.FailedNotConnected:
					return SendResult.FailedNotConnected;

				case NetSendResult.Sent:
					return SendResult.Sent;

				case NetSendResult.Queued:
					return SendResult.Queued;

				case NetSendResult.Dropped:
					return SendResult.Invalid;
				default:
					throw new InvalidOperationException($"Failed to map {nameof(NetSendResult)} value: {result} to GladnEt {nameof(SendResult)}.");
			}
		}
	}
}
