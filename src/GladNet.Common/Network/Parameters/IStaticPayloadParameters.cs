using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	/// <summary>
	/// Optional contract for <see cref="PacketPayload"/>s to implement if they have unchanging (static)
	/// parameters for their message sending. If they MUST be sent in such a way.
	/// </summary>
	public interface IStaticPayloadParameters : IMessageParameters
	{
		//bool VerifyAgainst(IMessageParameters parameters);

		//bool VerifyAgainst(bool encrypt, byte channel, DeliveryMethod method);
	}
}
