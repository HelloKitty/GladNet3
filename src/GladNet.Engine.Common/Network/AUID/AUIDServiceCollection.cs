using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GladNet.Engine.Common
{
	/// <summary>
	/// A non-thread safe, but syncronization ready, AUID collection service that maps <typeparamref name="TAUIDMapType"/> to
	/// their AUID <see cref="int"/> values.
	/// </summary>
	/// <typeparam name="TAUIDMapType"></typeparam>
	public class AUIDServiceCollection<TAUIDMapType> : Dictionary<int, TAUIDMapType>, IAUIDService<TAUIDMapType>
		where TAUIDMapType : class
	{
		/// <summary>
		/// Creates a new AUID map service collection that maps AUIDs to <typeparamref name="TAUIDMapType"/>s references.
		/// </summary>
		/// <param name="capacity">The initial number of elements that the service can contain.</param>
		public AUIDServiceCollection(int capacity)
			: base(capacity)
		{

		}

		/// <summary>
		/// Syncronationization object for accessing the dictionary.
		/// </summary>
		public ReaderWriterLockSlim syncObj { get; } = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
	}
}
