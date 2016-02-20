using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
	public interface IClassLogger
	{
		ILog Logger { get; }
	}
}
