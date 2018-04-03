using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using Common.Logging.Simple;

namespace GladNet
{
	public static class NetworkServerClientBuildingExtensions
	{
		/// <summary>
		/// Creates a managed client adapter around the provided <see cref="client"/> providing a high level API
		/// for consumption based on this simplified slimed down <see cref="IManagedNetworkServerClient{TPayloadWriteType,TPayloadReadType}"/>
		/// interface.
		/// </summary>
		/// <typeparam name="TReadPayloadBaseType">The read type payload (inferred)</typeparam>
		/// <typeparam name="TWritePayloadBaseType">The write type payload (inferred)</typeparam>
		/// <param name="client">The client to adapt.</param>
		/// <returns>A new managed client.</returns>
		public static IManagedNetworkServerClient<TWritePayloadBaseType, TReadPayloadBaseType> AsManagedSession<TReadPayloadBaseType, TWritePayloadBaseType>(this INetworkMessageClient<TReadPayloadBaseType, TWritePayloadBaseType> client)
			where TWritePayloadBaseType : class
			where TReadPayloadBaseType : class
		{
			if(client == null) throw new ArgumentNullException(nameof(client));

			//Adapt the provided network client to the managed network client interfaces.
			return AsManagedSession(client, new NoOpLogger());
		}

		/// <summary>
		/// Creates a managed client adapter around the provided <see cref="client"/> providing a high level API
		/// for consumption based on this simplified slimed down <see cref="IManagedNetworkServerClient{TPayloadWriteType,TPayloadReadType}"/>
		/// interface.
		/// </summary>
		/// <typeparam name="TReadPayloadBaseType">The read type payload (inferred)</typeparam>
		/// <typeparam name="TWritePayloadBaseType">The write type payload (inferred)</typeparam>
		/// <param name="client">The client to adapt.</param>
		/// <param name="logger"></param>
		/// <returns>A new managed client.</returns>
		public static IManagedNetworkServerClient<TWritePayloadBaseType, TReadPayloadBaseType> AsManagedSession<TReadPayloadBaseType, TWritePayloadBaseType>(this INetworkMessageClient<TReadPayloadBaseType, TWritePayloadBaseType> client, ILog logger)
			where TWritePayloadBaseType : class
			where TReadPayloadBaseType : class
		{
			if(client == null) throw new ArgumentNullException(nameof(client));
			if(logger == null) throw new ArgumentNullException(nameof(logger));

			//Adapt the provided network client to the managed network client interfaces.
			return new ManagedNetworkServerClient<INetworkMessageClient<TReadPayloadBaseType, TWritePayloadBaseType>, TWritePayloadBaseType, TReadPayloadBaseType>(client, logger);
		}
	}
}
