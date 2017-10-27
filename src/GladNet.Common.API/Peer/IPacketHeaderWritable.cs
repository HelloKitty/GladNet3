﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GladNet;

namespace GladNet
{
	/// <summary>
	/// Contract for client that exposed <see cref="IPacketHeader"/> writing
	/// capabilities.
	/// </summary>
	public interface IPacketHeaderWritable
	{
		/// <summary>
		/// Attempts to write a <see cref="IPacketHeader"/> to the
		/// client.
		/// </summary>
		/// <param name="header">The packet header to write.</param>
		/// <returns>An awaitable future that completes when the header has been written.</returns>
		Task WriteHeaderAsync(IPacketHeader header);
	}
}
