using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	/// <summary>
	/// <see cref="ResponseMessage"/>s are <see cref="NetworkMessage"/>s in response to <see cref="RequestMessage"/> from remote peers.
	/// It contains additional fields/properties compared to <see cref="NetworkMessage"/> that provide information on the response.
	/// </summary>
	public class ResponseMessage : NetworkMessage, IResponseMessage
	{
		/// <summary>
		/// Indicates the response state of the <see cref="ResponseMessage"/>.
		/// 0 - Generally Failure
		/// 1 - Generally Sucess
		/// 2+ - User-Defined
		/// </summary>
		public byte ResponseCode { get; private set; }

		/// <summary>
		/// Constructor for <see cref="ResponseMessage"/> that calls <see cref="NetworkMessage"/>.ctor
		/// </summary>
		/// <param name="payload"><see cref="PacketPayload"/> of the <see cref="NetworkMessage"/>.</param>
		/// <param name="rCode">Reponse code</param>
		public ResponseMessage(PacketPayload payload, byte rCode)
			: base(payload)
		{
			ResponseCode = rCode;
		}

		/// <summary>
		/// Dispatches the <see cref="ResponseMessage"/> (this) to the supplied <see cref="INetworkMessageReceiver"/>.
		/// </summary>
		/// <param name="receiver">The target <see cref="INetworkMessageReceiver"/>.</param>
		/// <exception cref="ArgumentNullException">Throws if either parameters are null.</exception>
		/// <param name="mParams">The <see cref="IMessageParameters"/> of the <see cref="ResponseMessage"/>.</param>
		public override void Dispatch(INetworkMessageReceiver receiver, IMessageParameters mParams)
		{
#if DEBUG || DEBUGBUILD
			if(receiver == null)
				throw new ArgumentNullException("receiver", typeof(INetworkMessageReceiver).ToString() + " parameter is null in " + GetType().ToString());

			if(mParams == null)
				throw new ArgumentNullException("mParams", typeof(IMessageParameters).ToString() + " parameter is null in " + GetType().ToString());
#endif

			receiver.OnNetworkMessageRecieve(this, mParams);
		}
	}
}
