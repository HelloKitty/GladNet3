using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GladNet.Lidgren.Server.Unity
{
	[Serializable]
	public class ConnectionInfo
	{
		[SerializeField]
		private string applicationIdentifier;

		public string ApplicationIdentifier => applicationIdentifier;

		[SerializeField]
		private int port;

		public int Port => port;
	}
}
