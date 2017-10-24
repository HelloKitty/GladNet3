using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GladNet
{
	public static class IConnectableExtensions
	{
		/// <summary>
		/// Connects to the provided <see cref="address"/> with on the given <see cref="port"/>.
		/// </summary>
		/// <param name="connectable"></param>
		/// <param name="address">The ip.</param>
		/// <param name="port">The port.</param>
		/// <returns>True if connection was successful.</returns>
		public static bool Connect(this IConnectable connectable, IPAddress address, int port)
		{
			if(connectable == null) throw new ArgumentNullException(nameof(connectable));
			if(address == null) throw new ArgumentNullException(nameof(address));
			if(port < 0) throw new ArgumentOutOfRangeException(nameof(port));

			return connectable.Connect(address.ToString(), port);
		}

		/// <summary>
		/// Connects to the provided <see cref="address"/> with on the given <see cref="port"/>.
		/// </summary>
		/// <param name="connectable"></param>
		/// <param name="address">The ip.</param>
		/// <param name="port">The port.</param>
		/// <returns>True if connection was successful.</returns>
		public static async Task<bool> ConnectAsync(this IConnectable connectable, IPAddress address, int port)
		{
			if(connectable == null) throw new ArgumentNullException(nameof(connectable));
			if(address == null) throw new ArgumentNullException(nameof(address));
			if(port < 0) throw new ArgumentOutOfRangeException(nameof(port));

			return await connectable.ConnectAsync(address.ToString(), port);
		}
	}
}
