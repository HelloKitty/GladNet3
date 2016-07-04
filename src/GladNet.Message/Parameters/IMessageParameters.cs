using GladNet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GladNet.Message
{
	/// <summary>
	/// Contract providing details about message parameters which are requested for processing
	/// </summary>
	public interface IMessageParameters
	{
		/// <summary>
		/// Indicates if the messge is/was encrypted depending on context.
		/// </summary>
		bool Encrypted { get; }

		/// <summary>
		/// Indicates the channel of the message.
		/// </summary>
		byte Channel { get; }

		//OP-Type removed. It is not needed and if it were there are other roundabount ways of computing it.

		/// <summary>
		/// Indicates the <see cref="DeliveryMethod"/> method of the message. Can/should be used to verify correct channel usage.
		/// </summary>
		DeliveryMethod DeliveryMethod { get; }
	}
}
