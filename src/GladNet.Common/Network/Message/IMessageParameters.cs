using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common
{
	/// <summary>
	/// Contract providing details about the parameters with which a message is sent or recieved with.
	/// </summary>
	public interface IMessageParameters
	{
		/// <summary>
		/// Indicates packet encryption.
		/// </summary>
		bool Encrypted { get; }

		/// <summary>
		/// Indicates the <see cref="NetworkMessage.OperationType"/> of a message.
		/// </summary>
		NetworkMessage.OperationType OpType { get; }

		/// <summary>
		/// Indicates the <see cref="NetworkMessage.DeliveryMethod"/> method of the message. Can be used to verify correct channel usage.
		/// </summary>
		NetworkMessage.DeliveryMethod DeliveryMethod { get; }
	}
}
