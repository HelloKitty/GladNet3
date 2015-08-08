using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	/// <summary>
	/// <see cref="RequestMessage"/>s are <see cref="NetworkMessage"/>s that request remote peers for/to-do something.
	/// Generally these ellict <see cref="ResponseMessage"/> but there is no implict mechanism in either <see cref="NetworkMessage"/>
	/// Subtypes for such a thing.
	/// </summary>
	public class RequestMessage : NetworkMessage, IRequestMessage
	{
		/// <summary>
		/// Constructor for <see cref="RequestMessage"/> that calls <see cref="NetworkMessage"/>.ctor
		/// </summary>
		/// <param name="payload"><see cref="PacketPayload"/> of the <see cref="NetworkMessage"/>.</param>
		public RequestMessage(PacketPayload payload, IRequestPayload parameters)
			: base(payload)
		{
			if (parameters == null)
				throw new ArgumentNullException("parameters", typeof(IRequestPayload) + " object of " + this.GetType() + " cannot be null in construction.");
		}

		/// <summary>
		/// Dispatches the <see cref="RequestMessage"/> (this) to the supplied <see cref="INetworkMessageReceiver"/>.
		/// </summary>
		/// <param name="receiver">The target <see cref="INetworkMessageReceiver"/>.</param>
		/// <exception cref="ArgumentNullException">Throws if either parameters are null.</exception>
		/// <param name="parameters">The <see cref="IMessageParameters"/> of the <see cref="RequestMessage"/>.</param>
		public override void Dispatch(INetworkMessageReceiver receiver, IMessageParameters parameters)
		{

#if DEBUG || DEBUGBUILD
			if(receiver == null)
				throw new ArgumentNullException("receiver", typeof(INetworkMessageReceiver).ToString() + " parameter is null in " + GetType().ToString());

			if(parameters == null)
				throw new ArgumentNullException("parameters", typeof(IMessageParameters).ToString() + " parameter is null in " + GetType().ToString());
#endif

			receiver.OnNetworkMessageReceive(this, parameters);
		}
	}
}
