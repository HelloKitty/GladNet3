using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GladNet.Lidgren.Server.Application
{
	/// <summary>
	/// Config loader for the <see cref="ServerConfiguration"/>
	/// </summary>
	public sealed class ServerConfigurationLoader
	{
		public ServerConfiguration LoadFromFile(string filePath)
		{
			if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(filePath));

			try
			{
				using (StreamReader file = File.OpenText(filePath))
				using (JsonTextReader reader = new JsonTextReader(file))
				{
					return JsonConvert.DeserializeObject<ServerConfiguration>(reader.ReadAsString());
				}
			}
			catch (Exception e)
			{
				throw new InvalidOperationException($"Could not load {nameof(ServerConfiguration)} file from the path: {filePath}.", e);
			}
		}
	}
}
