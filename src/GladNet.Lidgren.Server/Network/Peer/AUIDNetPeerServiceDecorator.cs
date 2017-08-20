using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using GladNet.Engine.Common;

namespace GladNet.Lidgren.Server
{
	/// <summary>
	/// Decorates the <see cref="IAUIDService{TAUIDMapType}"/> service for accessing the <see cref="INetPeer"/> behind it.
	/// </summary>
	public class AUIDNetPeerServiceDecorator : IAUIDService<INetPeer>
	{
		/// <summary>
		/// Decorated AUID service.
		/// </summary>
		private IAUIDService<ClientSessionServiceContext> lidgrenServiceContextAUIDService { get; }

		public AUIDNetPeerServiceDecorator(IAUIDService<ClientSessionServiceContext> auidServiceContext)
		{
			if (auidServiceContext == null)
				throw new ArgumentNullException(nameof(auidServiceContext), $"Provided {nameof(IAUIDService<ClientSessionServiceContext>)} must not be null.");

			lidgrenServiceContextAUIDService = auidServiceContext;
		}

		public INetPeer this[int key]
		{
			get => lidgrenServiceContextAUIDService[key]?.ClientNetPeer;

			set => throw new InvalidOperationException("Cannot set netpeer through netpeer decoractor.");
		}

		public int Count => lidgrenServiceContextAUIDService.Count;

		public bool IsReadOnly => lidgrenServiceContextAUIDService.IsReadOnly;

		public ICollection<int> Keys => lidgrenServiceContextAUIDService.Keys;

		public ReaderWriterLockSlim syncObj => lidgrenServiceContextAUIDService.syncObj;

		public ICollection<INetPeer> Values => lidgrenServiceContextAUIDService.Values.Select(x => x.ClientNetPeer).ToList();

		public void Add(KeyValuePair<int, INetPeer> item)
		{
			throw new InvalidOperationException($"Cannot {nameof(Add)} netpeer through netpeer decoractor.");
		}

		public void Add(int key, INetPeer value)
		{
			throw new InvalidOperationException($"Cannot {nameof(Add)} netpeer through netpeer decoractor.");
		}

		public void Clear()
		{
			lidgrenServiceContextAUIDService.Clear();
		}

		public bool Contains(KeyValuePair<int, INetPeer> item)
		{
			return this.ContainsKey(item.Key) && lidgrenServiceContextAUIDService[item.Key]?.ClientNetPeer == item.Value;
		}

		public bool ContainsKey(int key)
		{
			return lidgrenServiceContextAUIDService.ContainsKey(key);
		}

		public void CopyTo(KeyValuePair<int, INetPeer>[] array, int arrayIndex)
		{
			throw new InvalidOperationException($"Cannot {nameof(CopyTo)} netpeer through netpeer decoractor.");
		}

		//Try to never call this
		public IEnumerator<KeyValuePair<int, INetPeer>> GetEnumerator()
		{
			return lidgrenServiceContextAUIDService.Select(x => new KeyValuePair<int, INetPeer>(x.Key, x.Value.ClientNetPeer)).GetEnumerator();
		}

		public bool Remove(KeyValuePair<int, INetPeer> item)
		{
			if (Contains(item))
				return lidgrenServiceContextAUIDService.Remove(item.Key);
			else
				return false;
		}

		public bool Remove(int key)
		{
			return lidgrenServiceContextAUIDService.Remove(key);
		}

		public bool TryGetValue(int key, out INetPeer value)
		{
			value = ContainsKey(key) ? lidgrenServiceContextAUIDService[key]?.ClientNetPeer : null;

			return value != null;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
