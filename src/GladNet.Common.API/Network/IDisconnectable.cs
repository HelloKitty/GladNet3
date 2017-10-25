using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GladNet
{
	public interface IDisconnectable
	{
		/// <summary>
		/// Disconnects asyncronously and allows the client <see cref="delay"/> many milliseconds
		/// to send or recieve remaining data.
		/// </summary>
		/// <param name="delay">The delay to allow for final sending.</param>
		/// <returns>A wait.</returns>
		Task DisconnectAsync(int delay);
	}
}
