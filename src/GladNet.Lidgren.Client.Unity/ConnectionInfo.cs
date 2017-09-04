using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

namespace GladNet.Lidgren.Client.Unity
{
	[Serializable]
	public class ConnectionInfo
	{
		[SerializeField]
		private string applicationIdentifier;

		public string ApplicationIdentifier => applicationIdentifier;

		[SerializeField]
		private int remotePort;

		public int RemotePort => remotePort;

		[SerializeField]
		private string serverIp;

		public string ServerIp => serverIp;

		public ConnectionInfo([NotNull] string appId, int port, [NotNull] string ip)
		{
			if(string.IsNullOrEmpty(appId)) throw new ArgumentException("Value cannot be null or empty.", nameof(appId));
			if(port < 0) throw new ArgumentOutOfRangeException(nameof(port));
			if(string.IsNullOrEmpty(ip)) throw new ArgumentException("Value cannot be null or empty.", nameof(ip));

			applicationIdentifier = appId;
			remotePort = port;
			serverIp = ip;
		}
	}
}
