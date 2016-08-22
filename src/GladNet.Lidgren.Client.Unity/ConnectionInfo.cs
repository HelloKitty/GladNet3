using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GladNet.Lidgren.Client.Unity
{
	[Serializable]
	public class ConnectionInfo
	{
		[SerializeField]
		private string applicationIdentifier;

		public string ApplicationIdentifier { get { return applicationIdentifier; } }

		[SerializeField]
		private int remotePort;

		public int RemotePort { get { return remotePort; } }

		[SerializeField]
		private string serverIp;

		public string ServerIp { get { return serverIp; } }
	}
}
