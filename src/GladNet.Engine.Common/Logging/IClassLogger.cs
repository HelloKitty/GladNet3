using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Engine.Common
{
	/// <summary>
	/// Contract for class that provides externally visible logging services.
	/// </summary>
	public interface IClassLogger
	{
		/// <summary>
		/// Class logging service.
		/// </summary>
		ILog Logger { get; }
	}
}
