using Easyception;
using GladNet.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Message.Handlers
{
	/// <summary>
	/// Extension methods for collections of <see cref="IMessageHandler{TPeerType, TNetworkMessageType}"/>.
	/// </summary>
	public static class PayloadCollectionBuilderExtensions
	{

		/// <summary>
		/// Builds a <see cref="ChainMessageHandlerStrategy{TPeerType, TNetworkMessageType}"/> with the provided collections
		/// of <see cref="IMessageHandler{TPeerType, TNetworkMessageType}"/>s.
		/// </summary>
		/// <typeparam name="TPeerType">Peer Type (inferred usually)</typeparam>
		/// <typeparam name="TNetworkMessageType">Network message type (inferred usually)</typeparam>
		/// <param name="handlers">Handlers to use to build the chain handler.</param>
		/// <returns>A new chain handler strategy.</returns>
		public static ChainMessageHandlerStrategy<TPeerType, TNetworkMessageType> ToChainHandler<TPeerType, TNetworkMessageType>(this IEnumerable<IMessageHandler<TPeerType, TNetworkMessageType>> handlers)
			where TPeerType : INetPeer
			where TNetworkMessageType : INetworkMessage
		{
			Throw<ArgumentNullException>.If.IsNull(handlers)?.Now(nameof(handlers), $"Cannot build a {nameof(ChainMessageHandlerStrategy<TPeerType, TNetworkMessageType>)} with a null collection of {nameof(IMessageHandler<TPeerType, TNetworkMessageType>)}");

			return new ChainMessageHandlerStrategy<TPeerType, TNetworkMessageType>(handlers);
		}
	}
}
