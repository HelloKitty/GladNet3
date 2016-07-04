using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Engine.Common
{
	/// <summary>
	/// Contract for implementing disconnectable functionality.
	/// </summary>
	public interface IDisconnectable
	{
		/// <summary>
		/// Disconnects the disconnectable object.
		/// </summary>
		void Disconnect();
	}
}
