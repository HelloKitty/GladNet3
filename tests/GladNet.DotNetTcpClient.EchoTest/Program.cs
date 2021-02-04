using System;
using System.IO.Pipelines;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Glader.Essentials;
using Pipelines.Sockets.Unofficial;

namespace GladNet.DotNetTcpClient.EchoTest
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.ReadKey();
			var socket = new TCPSocketConnectionFactory()
				.Create();

			await socket.Socket.ConnectAsync(IPAddress.Parse("127.0.0.1"), 6969);

			if (!socket.Socket.Connected)
				throw new InvalidOperationException($"Failed to connect.");

			NetworkConnectionOptions options = new NetworkConnectionOptions(2, 2, 1024);
			var serializer = new StringMessageSerializer();

			SessionMessageBuildingServiceContext<string, string> messageServices =
				new SessionMessageBuildingServiceContext<string, string>(new StringMessagePacketHeaderFactory(), serializer, serializer, new StringPacketHeaderSerializer());

			//Build the message disaptching strategy, for how and where and in what way messages will be handled
			var handlerService = new DefaultMessageHandlerService<string, SessionMessageContext<string>>();
			var dispatcher = new InPlaceNetworkMessageDispatchingStrategy<string, string>(handlerService);

			//Bind one of the default handlers
			handlerService.Bind<string>(new DefaultStringMessageHandler());

			//We build the session context 1 time because it should not change
			//Rather than inject as a dependency, we can build it in here. Really optional to inject
			//or build it in CTOR
			//Either way we build a send service and session context that captures it to provide to handling.
			SocketConnectionNetworkMessageInterface<string, string> messageInterface = new SocketConnectionNetworkMessageInterface<string, string>(options, socket, messageServices);
			var session = new TCPEchoClientSession(options, socket, new SessionDetails(new NetworkAddressInfo(IPAddress.Parse("127.0.0.1"), 6969), 1), messageServices, dispatcher, messageInterface);

			session.AttachDisposable(socket);
			session.AttachDisposable(socket.Socket);

			SessionStarter<TCPEchoClientSession> starter = new SessionStarter<TCPEchoClientSession>(new ConsoleLogger(LogLevel.All, true));

			Task.Run(async () =>
			{
				while (session.ConnectionService.isConnected)
				{
					Console.Write($"Enter: ");
					string input = Console.ReadLine();
					await messageInterface.SendMessageAsync(input);
				}
			});

			await starter.StartAsync(session, CancellationToken.None);
		}
	}
}
