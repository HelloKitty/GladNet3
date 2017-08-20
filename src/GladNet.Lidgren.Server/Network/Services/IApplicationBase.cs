using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GladNet.Serializer;
using Lidgren.Network;

namespace GladNet.Lidgren.Server
{
	public interface IApplicationBase
	{
		/// <summary>
		/// Starts the application server listener.
		/// </summary>
		/// <param name="netConfig">The configuration for starting the server.</param>
		void StartServer(NetPeerConfiguration netConfig);

		/// <summary>
		/// Stops the application server listener.
		/// </summary>
		void StopServer();

		/// <summary>
		/// Registers <see cref="Type"/>s with the serializer registry.
		/// </summary>
		/// <param name="registry"></param>
		void RegisterTypes(ISerializerRegistry registry);
	}
}
