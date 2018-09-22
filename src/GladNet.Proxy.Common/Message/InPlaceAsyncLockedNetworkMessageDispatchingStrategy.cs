using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GladNet;
using Nito.AsyncEx;

namespace GladNet
{
	public sealed class InPlaceAsyncLockedNetworkMessageDispatchingStrategy<TPayloadWriteType, TPayloadReadType> : INetworkMessageDispatchingStrategy<TPayloadWriteType, TPayloadReadType>
		where TPayloadWriteType : class
		where TPayloadReadType : class
	{
		//TODO: Inject this instead? Make this a strategy decorator?
		private InPlaceNetworkMessageDispatchingStrategy<TPayloadWriteType, TPayloadReadType> DecoratedDisaDispatchingStrategy { get; }

		public InPlaceAsyncLockedNetworkMessageDispatchingStrategy()
		{
			DecoratedDisaDispatchingStrategy = new InPlaceNetworkMessageDispatchingStrategy<TPayloadWriteType, TPayloadReadType>();
		}

		public async Task DispatchNetworkMessage(SessionMessageContext<TPayloadWriteType, TPayloadReadType> context)
		{
			//TODO: Renable lock
			//using(await InPlaceAsyncLockedNetworkMessageDispatchingStrategy.LockObject.LockAsync().ConfigureAwait(false))
				await DecoratedDisaDispatchingStrategy.DispatchNetworkMessage(context)
					.ConfigureAwait(false);
		}
	}

	/// <summary>
	/// Non-generic <see cref="InPlaceNetworkMessageDispatchingStrategy{TPayloadWriteType,TPayloadReadType}"/>
	/// for static properties/fields.
	/// </summary>
	internal sealed class InPlaceAsyncLockedNetworkMessageDispatchingStrategy
	{
		/// <summary>
		/// Async lock.
		/// </summary>
		internal static AsyncLock LockObject { get; } = new AsyncLock();
	}
}
