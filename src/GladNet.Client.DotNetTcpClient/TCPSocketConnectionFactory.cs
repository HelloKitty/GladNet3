using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Glader.Essentials;
using Pipelines.Sockets.Unofficial;

namespace GladNet
{
	/// <summary>
	/// Factory that creates <see cref="SocketConnection"/>.
	/// Warning: You are responsible for calling Dispose.
	/// </summary>
	public sealed class TCPSocketConnectionFactory : IFactoryCreatable<SocketConnection, EmptyFactoryContext>
	{
		/// <inheritdoc />
		public SocketConnection Create(EmptyFactoryContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			Socket client = new Socket(SocketType.Stream, ProtocolType.Tcp);

			//Let the caller connect if they want to.
			//await client.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6969));

			return SocketConnection.Create(client, PipeOptions.Default, PipeOptions.Default, SocketConnectionOptions.ZeroLengthReads);
		}
	}
}
