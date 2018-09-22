using System;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;
using JetBrains.Annotations;

namespace GladNet
{
	/// <summary>
	/// Simplied type alias for game handlers that handle Server payloads sent from the server.
	/// </summary>
	[ServerPayloadHandler]
	public abstract class BaseGameServerPayloadHandler<TSpecificPayloadType, TBasePayloadType, TOutgoingPayloadType> : BaseGamePayloadHandler<TSpecificPayloadType, TBasePayloadType, TOutgoingPayloadType>
		where TSpecificPayloadType : class, TBasePayloadType
		where TBasePayloadType : class, IPacketPayload
		where TOutgoingPayloadType : class, IPacketPayload
	{
		/// <inheritdoc />
		protected BaseGameServerPayloadHandler([NotNull] ILog logger) 
			: base(logger)
		{

		}
	}
}