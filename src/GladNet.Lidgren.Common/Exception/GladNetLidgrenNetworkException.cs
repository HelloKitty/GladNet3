using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace GladNet.Lidgren
{
	public sealed class GladNetLidgrenNetworkException : Exception
	{
		/// <summary>
		/// The connection associated with the network exception.
		/// </summary>
		public NetConnection Connection { get; }

		/// <summary>
		/// Indicates if there is a base exception present in the exception.
		/// </summary>
		public bool hasBaseException => BaseException != null;

		/// <summary>
		/// The optional base exception
		/// </summary>
		public Exception BaseException { get; }

		public GladNetLidgrenNetworkException(string message, NetConnection connection, Exception baseException = null) 
			: base(message)
		{
			if (connection == null) throw new ArgumentNullException(nameof(connection));

			Connection = connection;
			BaseException = baseException;
		}
	}
}
