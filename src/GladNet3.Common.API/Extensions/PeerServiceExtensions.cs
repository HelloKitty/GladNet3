using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GladNet
{
	public static class PeerServiceExtensions
	{
		/// <summary>
		/// Produces a <see cref="NetworkIncomingMessage{TPayloadType}"/> asyncronously.
		/// The task will complete when a network message is available.
		/// </summary>
		/// <returns>Returns a future that will complete when a message is available.</returns>
		public static Task<NetworkIncomingMessage<TPayloadBaseType>> ReadMessageAsync<TPayloadBaseType>(this INetworkMessageProducer<TPayloadBaseType> producer) 
			where TPayloadBaseType : class
		{
			if(producer == null) throw new ArgumentNullException(nameof(producer));

			return producer.ReadMessageAsync(CancellationToken.None);
		}
	}
}
