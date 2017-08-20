using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GladNet.Lidgren.Server.Application
{
	/// <summary>
	/// Bootstrapping application that loads the ApplicationBase and hands off control.
	/// </summary>
	public class Program
	{
		private static void Main(string[] args)
		{
			ServerConfigurationLoader configLoader = new ServerConfigurationLoader();

			ServerConfiguration configuration = configLoader.LoadFromFile("config/ServerConfiguration.json");

			if(configuration == null)
				throw new InvalidOperationException($"Could not load the configuration file.");

			//Now we must load
		}
	}
}
