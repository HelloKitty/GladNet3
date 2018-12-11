using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Common.Logging;
using GladNet;
using JetBrains.Annotations;

namespace GladNet
{
	public abstract class ProxiedTcpServerApplicationBase<TPayloadWriteType, TPayloadReadType> : TcpServerServerApplicationBase<TPayloadWriteType, TPayloadReadType> 
		where TPayloadWriteType : class where TPayloadReadType : class
	{
		/// <summary>
		/// The endpoint that this proxy application is sitting in the middle of.
		/// </summary>
		public NetworkAddressInfo ProxyToEndpointAddress { get; }

		private PayloadHandlerRegisterationModules<TPayloadReadType, TPayloadWriteType> HandlerModulePair { get; }

		private NetworkSerializerServicePair Serializers { get; }

		protected IContainer ServiceContainer { get; }

		/// <inheritdoc />
		protected ProxiedTcpServerApplicationBase(NetworkAddressInfo listenerAddress, [NotNull] NetworkAddressInfo proxyToEndpointAddress,  [NotNull] ILog logger, PayloadHandlerRegisterationModules<TPayloadReadType, TPayloadWriteType> handlerModulePair, NetworkSerializerServicePair serializers) 
			: this(listenerAddress, proxyToEndpointAddress, logger, new InPlaceAsyncLockedNetworkMessageDispatchingStrategy<TPayloadWriteType, TPayloadReadType>(), handlerModulePair, serializers)
		{
			
		}

		/// <inheritdoc />
		private ProxiedTcpServerApplicationBase([NotNull] NetworkAddressInfo serverAddress, [NotNull] NetworkAddressInfo proxyToEndpointAddress, [NotNull] ILog logger, [NotNull] INetworkMessageDispatchingStrategy<TPayloadWriteType, TPayloadReadType> messageHandlingStrategy, [NotNull] PayloadHandlerRegisterationModules<TPayloadReadType, TPayloadWriteType> handlerModulePair, [NotNull] NetworkSerializerServicePair serializers) 
			: base(serverAddress, messageHandlingStrategy, logger)
		{
			HandlerModulePair = handlerModulePair ?? throw new ArgumentNullException(nameof(handlerModulePair));
			Serializers = serializers ?? throw new ArgumentNullException(nameof(serializers));
			ProxyToEndpointAddress = proxyToEndpointAddress ?? throw new ArgumentNullException(nameof(proxyToEndpointAddress));
			ServiceContainer = BuildServiceContainer();
		}

		/// <inheritdoc />
		protected override bool IsClientAcceptable(TcpClient tcpClient)
		{
			return true;
		}

		/// <inheritdoc />
		protected override IManagedNetworkServerClient<TPayloadWriteType, TPayloadReadType> CreateIncomingSessionPipeline(TcpClient client)
		{
			//TODO: Are any details actually valuable here?
			if(Logger.IsInfoEnabled)
				Logger.Info($"Client connected to proxy.");

			return BuildIncomingSessionManagedClient(new DotNetTcpClientNetworkClient(client), Serializers.ProxiedClientSerializer);
		}

		private IManagedNetworkClient<TPayloadReadType, TPayloadWriteType> CreateOutgoingSessionPipeline(TcpClient client)
		{
			if(Logger.IsInfoEnabled)
				Logger.Info($"Creating outgoing proxy client.");

			return BuildOutgoingSessionManagedClient(new DotNetTcpClientNetworkClient(client), Serializers.ProxiedServerSerializer);
		}

		protected override ManagedClientSession<TPayloadWriteType, TPayloadReadType> CreateIncomingSession(IManagedNetworkServerClient<TPayloadWriteType, TPayloadReadType> client, SessionDetails details)
		{
			Logger.Info($"Recieved proxy connection from: {details.Address.AddressEndpoint.ToString()}:{details.Address.Port}");

			TcpClient proxyClientTcpClient = new TcpClient(ProxyToEndpointAddress.AddressEndpoint.ToString(), ProxyToEndpointAddress.Port);

			//We need to create the proxy client now too
			var proxyClient = CreateOutgoingSessionPipeline(proxyClientTcpClient);

			//We need to use AutoFac lifetime scope so we can register the individual specific dependencies for the
			//session
			GenericProxiedManagedClientSession<TPayloadWriteType, TPayloadReadType> connectionSession = BuildSessionFromDependencies<TPayloadWriteType, TPayloadReadType>(client, details, proxyClient);

			//After the connection session is made with the message context factory that has a dependency on the proxyclient we must create the proxy client's session
			//which makes it easier to manage and it will have a dependency on the actual session
			GenericProxiedManagedClientSession<TPayloadReadType, TPayloadWriteType> clientProxySession = BuildSessionFromDependencies<TPayloadReadType, TPayloadWriteType>(proxyClient, details, client);

			//Now they can both communicate between eachother through the handler's message contexts
			//However since the AppBase only takes one session type, to maintain this session we need to manually start it
			//with the ManualClientConnectionLoop below. A copy-paste from the AppBase.
			Task.Factory.StartNew(async () => { await ManualStartClientConnectionLoop(proxyClientTcpClient, proxyClient, clientProxySession).ConfigureAwait(false); })
				.ConfigureAwait(false);

			return connectionSession;
		}

		private GenericProxiedManagedClientSession<TWriteType, TReadType> BuildSessionFromDependencies<TWriteType, TReadType>(IManagedNetworkClient<TWriteType, TReadType> client, SessionDetails details, IManagedNetworkClient<TReadType, TWriteType> proxyClient)
			where TReadType : class 
			where TWriteType : class
		{
			GenericProxiedManagedClientSession<TWriteType, TReadType> connectionSession;
			using(ILifetimeScope lifetimeScope = this.ServiceContainer.BeginLifetimeScope(c =>
			{
				c.RegisterInstance(client)
					.AsImplementedInterfaces()
					.AsSelf();

				c.RegisterInstance(details)
					.As<SessionDetails>();

				c.RegisterInstance(new GenericMessageContextFactory<TWriteType, TReadType>(proxyClient))
					.AsSelf()
					.AsImplementedInterfaces();
			}))
			{
				connectionSession = GenerateClientFromLifetimeScope<TWriteType, TReadType>(lifetimeScope);
			}

			return connectionSession;
		}

		protected virtual GenericProxiedManagedClientSession<TWriteType, TReadType> GenerateClientFromLifetimeScope<TWriteType, TReadType>(ILifetimeScope lifetimeScope)
			where TWriteType : class
			where TReadType : class
		{
			//TODO: Whenever a client session is created we should create a parallel client connection to the server we're in the middle of
			return lifetimeScope.Resolve<GenericProxiedManagedClientSession<TWriteType, TReadType>>();
		}

		/// <summary>
		/// Implementer should build the <see cref="IManagedNetworkServerClient{TPayloadWriteType,TPayloadReadType}"/>
		/// for the incoming session.
		/// </summary>
		/// <param name="clientBase">The client base to use.</param>
		/// <param name="serializeService">The serializer to use.</param>
		/// <returns>The built managed client.</returns>
		protected abstract IManagedNetworkServerClient<TPayloadWriteType, TPayloadReadType> BuildIncomingSessionManagedClient(NetworkClientBase clientBase, INetworkSerializationService serializeService);

		/// <summary>
		/// Implementer should build the <see cref="IManagedNetworkClient{TPayloadWriteType,TPayloadReadType}"/>
		/// for the outgoing client.
		/// </summary>
		/// <param name="clientBase">The client base to use.</param>
		/// <param name="serializeService">The serializer to use.</param>
		/// <returns>The built managed client.</returns>
		protected abstract IManagedNetworkClient<TPayloadReadType, TPayloadWriteType> BuildOutgoingSessionManagedClient(NetworkClientBase clientBase, INetworkSerializationService serializeService);

		//TODO: We don't need direct dependency on AutoFac. We should use new ASP Core ServiceCollection so others can be used.
		private IContainer BuildServiceContainer()
		{
			ContainerBuilder builder = new ContainerBuilder();

			//TODO: Register serializers somehow
			//TODO: Handle default handlers

			builder.RegisterInstance(Logger)
				.As<ILog>();

			RegisterMessageHandlerServices(builder);

			RegisterSessionTypes(builder);

			//Register the server and client handler modules
			HandlerModulePair.ClientMessageHandlerModules
				.ToList()
				.ForEach(m => builder.RegisterModule(m));
			HandlerModulePair.ServerMessageHandlerModules
				.ToList()
				.ForEach(m => builder.RegisterModule(m));

			builder = RegisterHandlerDependencies(builder);

			//The default handlers (Just forwards)
			builder = RegisterDefaultHandlers(builder);

			return builder.Build();
		}

		protected virtual void RegisterSessionTypes(ContainerBuilder builder)
		{
			//Register the incoming and outgoing session Types.
			builder.RegisterType<GenericProxiedManagedClientSession<TPayloadWriteType, TPayloadReadType>>()
				.AsSelf();

			builder.RegisterType<GenericProxiedManagedClientSession<TPayloadReadType, TPayloadWriteType>>()
				.AsSelf();
		}

		protected virtual void RegisterMessageHandlerServices(ContainerBuilder builder)
		{
			//The session handlers
			builder.RegisterType<MessageHandlerService<TPayloadWriteType, TPayloadReadType, IProxiedMessageContext<TPayloadReadType, TPayloadWriteType>>>()
				.As<MessageHandlerService<TPayloadWriteType, TPayloadReadType, IProxiedMessageContext<TPayloadReadType, TPayloadWriteType>>>()
				.SingleInstance();

			//The proxy client handlers
			builder.RegisterType<MessageHandlerService<TPayloadReadType, TPayloadWriteType, IProxiedMessageContext<TPayloadWriteType, TPayloadReadType>>>()
				.As<MessageHandlerService<TPayloadReadType, TPayloadWriteType, IProxiedMessageContext<TPayloadWriteType, TPayloadReadType>>>()
				.SingleInstance();
		}

		//TODO: We don't need direct dependency on AutoFac. We should use new ASP Core ServiceCollection so others can be used.
		/// <summary>
		/// Implementers should register all dependencies the handlers might need.
		/// Excluding: <see cref="ILog"/>, <see cref="INetworkSerializationService"/> and <see cref="NetworkSerializerServicePair"/>.
		/// </summary>
		/// <param name="builder"></param>
		protected abstract ContainerBuilder RegisterHandlerDependencies(ContainerBuilder builder);

		private async Task ManualStartClientConnectionLoop(TcpClient client, IManagedNetworkClient<TPayloadReadType, TPayloadWriteType> internalNetworkClient, ManagedClientSession<TPayloadReadType, TPayloadWriteType> networkSession)
		{
			//So that sessions invoking the disconnection can internally disconnect to
			networkSession.OnSessionDisconnection += async (source, args) => await internalNetworkClient.DisconnectAsync(0).ConfigureAwait(false);

			//TODO: Better way to syncronize the strategies used?
			var dispatchingStrategy = new InPlaceAsyncLockedNetworkMessageDispatchingStrategy<TPayloadReadType, TPayloadWriteType>();

			while(client.Connected && internalNetworkClient.isConnected)
			{
				NetworkIncomingMessage<TPayloadWriteType> message = await internalNetworkClient.ReadMessageAsync(CancellationToken.None)
					.ConfigureAwait(false);

				//We don't want to stop the client just because an exception occurred.
				try
				{
					//TODO: This will work for World of Warcraft since it requires no more than one packet
					//from the same client be handled at one time. However it limits throughput and maybe we should
					//handle this at a different level instead. 
					await dispatchingStrategy.DispatchNetworkMessage(new SessionMessageContext<TPayloadReadType, TPayloadWriteType>(networkSession, message))
						.ConfigureAwait(false);
				}
				catch(Exception e)
				{
					//TODO: Remove this console log
					Logger.Error($"[Error]: {e.Message}\n\nStack: {e.StackTrace}");
				}
			}

			client.Dispose();

			//TODO: Should we tell the client something when it ends?
			await networkSession.DisconnectClientSession()
				.ConfigureAwait(false);
		}

		protected abstract ContainerBuilder RegisterDefaultHandlers(ContainerBuilder builder);
	}
}
