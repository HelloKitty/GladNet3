using GladNet.Common;
using GladNet.Message;
using GladNet.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Lidgren.Engine.Common
{
	public class LidgrenNetworkMessageFactory : INetworkMessageFactory
	{
		/// <summary>
		/// Creates a new <see cref="INetworkMessage"/>
		/// </summary>
		/// <param name="opType">Operation type of the message.</param>
		/// <param name="payload">Payload for the <see cref="INetworkMessage"/>.</param>
		/// <returns>A new non-null <see cref="INetworkMessage"/>.</returns>
		public INetworkMessage Create(OperationType opType, PacketPayload payload)
		{
			switch (opType)
			{
				case OperationType.Event:
					return new EventMessage(payload);
				case OperationType.Request:
					return new RequestMessage(payload);
				case OperationType.Response:
					return new ResponseMessage(payload);
				default:
					throw new InvalidOperationException($"Cannot create a {nameof(INetworkMessage)} instance for {nameof(OperationType)}: {opType}.");
			}
		}
	}
}
