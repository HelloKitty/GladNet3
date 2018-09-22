using System;
using System.Collections.Generic;
using System.Text;
using GladNet;
using JetBrains.Annotations;

namespace GladNet
{
	public class NetworkSerializerServicePair
	{
		/// <summary>
		/// The serializer used for proxying the client connection.
		/// </summary>
		public INetworkSerializationService ProxiedClientSerializer { get; }

		/// <summary>
		/// The serializer used for proxying the server connection.
		/// </summary>
		public INetworkSerializationService ProxiedServerSerializer { get; }

		/// <inheritdoc />
		public NetworkSerializerServicePair([NotNull] INetworkSerializationService proxiedClientSerializer, [NotNull] INetworkSerializationService proxiedServerSerializer)
		{
			ProxiedClientSerializer = proxiedClientSerializer ?? throw new ArgumentNullException(nameof(proxiedClientSerializer));
			ProxiedServerSerializer = proxiedServerSerializer ?? throw new ArgumentNullException(nameof(proxiedServerSerializer));
		}
	}
}
