using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GladNet.Engine.Common
{
	/// <summary>
	/// AUID mapping service for <typeparamref name="TAUIDMapType"/>.
	/// </summary>
	public interface IAUIDService<TAUIDMapType> : IDictionary<int, TAUIDMapType>
		where TAUIDMapType : class //just don't want people using this and being bad with GC. No real reason other than that.
	{
		/// <summary>
		/// Syncronationization object for accessing the dictionary.
		/// </summary>
		ReaderWriterLockSlim syncObj { get; }
	}
}
