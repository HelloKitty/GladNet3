using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;
using JetBrains.Annotations;

namespace GladNet
{
	/// <summary>
	/// Simplied type alias for game handlers that handle Client payloads sent from the client.
	/// </summary>
	[ClientPayloadHandler]
	public abstract class BaseGameClientPayloadHandler<TSpecificPayloadType, TBasePayloadType, TOutgoingPayloadType> : BaseGamePayloadHandler<TSpecificPayloadType, TBasePayloadType, TOutgoingPayloadType>
		where TSpecificPayloadType : class, TBasePayloadType
		where TBasePayloadType : class, IPacketPayload
		where TOutgoingPayloadType : class, IPacketPayload
	{
		/// <inheritdoc />
		protected BaseGameClientPayloadHandler([NotNull] ILog logger) 
			: base(logger)
		{

		}
	}
}
