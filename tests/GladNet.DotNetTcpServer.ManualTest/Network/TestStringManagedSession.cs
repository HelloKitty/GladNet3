using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pipelines.Sockets.Unofficial;

namespace GladNet
{
	public sealed class TestStringManagedSession : BaseTcpManagedSession<string, string>
	{
		public TestStringManagedSession(NetworkConnectionOptions networkOptions, SocketConnection connection, SessionDetails details, SessionMessageBuildingServiceContext<string, string> messageServices) 
			: base(networkOptions, connection, details, messageServices)
		{

		}

		/// <inheritdoc />
		public override async Task OnNetworkMessageReceivedAsync(NetworkIncomingMessage<string> message, CancellationToken token = default)
		{
			Console.WriteLine($"Message Content: {message.Payload}");

			//echos back the message to the client.
			await NetworkMessageInterface.SendMessageAsync(message.Payload, token);

			if (message.Payload.ToLower() == "quit")
				await ConnectionService.DisconnectAsync();
		}
	}
}
