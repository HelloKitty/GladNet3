using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GladNet.Lidgren.Server.Application
{
	[JsonObject]
	public sealed class ServerConfiguration
	{
		/// <summary>
		/// The IP or domain name the server should register itself on.
		/// </summary>
		[JsonProperty]
		public string ServerAddress { get; }

		/// <summary>
		/// The port for the server.
		/// </summary>
		[JsonProperty]
		public int ServerPort { get; }

		/// <summary>
		/// The application identifier.
		/// </summary>
		[JsonProperty]
		public string ApplicationName { get; }

		/// <summary>
		/// The name of the .NET assembly the server application is contained within.
		/// </summary>
		[JsonProperty]
		public string ServerAssemblyName { get; }
	}
}
