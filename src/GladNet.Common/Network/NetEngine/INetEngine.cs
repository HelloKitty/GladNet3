using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GladNet.Common
{
	/// <summary>
	/// Contract for implementing types to offer Network Engine functionality.
	/// Such as; network message sending, network details, connection details and more.
	/// </summary>
	public interface INetEngine : INetworkMessageSender
	{
		/// <summary>
		/// Provides immutable access to the details about a given connection the <see cref="INetEngine"/> is built on.
		/// </summary>
		IConnectionDetails Details { get; }
	}
}
