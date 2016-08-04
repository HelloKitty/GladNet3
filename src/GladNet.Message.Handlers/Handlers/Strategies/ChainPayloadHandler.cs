using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladLive.Common
{
	/// <summary>
	/// Provides chain of responsibility semantics as a strategy for payload handling.
	/// </summary>
	/// <typeparam name="TSessionType">Session type to handle.</typeparam>
	public class ChainPayloadHandler<TSessionType> : IPayloadHandlerStrategy<TSessionType>, IPayloadHandlerRegistry<TSessionType>
		where TSessionType : INetPeer
	{
		/// <summary>
		/// Collection of handles to chain over.
		/// </summary>
		private IList<IPayloadHandler<TSessionType>> handlers { get; }

		public ChainPayloadHandler()
		{
			handlers = new List<IPayloadHandler<TSessionType>>();
		}

		public ChainPayloadHandler(IEnumerable<IPayloadHandler<TSessionType>> handlersToChain)
		{
			handlers = new List<IPayloadHandler<TSessionType>>(handlersToChain);
		}

		public bool Register<THandlerPeerType, TPayloadType>(IPayloadHandler<THandlerPeerType, TPayloadType> payloadHandler)
			where THandlerPeerType : TSessionType
			where TPayloadType : PacketPayload
		{
			IPayloadHandler<TSessionType> h = payloadHandler as IPayloadHandler<TSessionType>;

			//Adds the handler to the collection
			//In the future we can do fancier things like checking to see if it has already been registered
			//We can also maybe lock to prepare for multithreading
			if (h != null)
			{
				handlers.Add(h);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Attempts to handle the <typeparamref name="TPayloadType"/> with static parameters.
		/// </summary>
		/// <typeparam name="TPayloadType">Payload type.</typeparam>
		/// <param name="payload">Payload instance.</param>
		/// <param name="parameters">Parameters the message was sent with.</param>
		/// <param name="peer">Peer that is involved with the message.</param>
		/// <returns>True if the handler can handle the type of packet.</returns>
		public virtual bool TryProcessPayload(PacketPayload payload, IMessageParameters parameters, TSessionType peer)
		{
			bool result = false;

			foreach(IPayloadHandler<TSessionType> h in handlers)
			{
				result = h.TryProcessPayload(payload, parameters, peer) || result;

				//Added consumption to the chain making the payloads handleable by only a single handler
				if (result)
					return true;
			}

			return result;
		}
	}
}
