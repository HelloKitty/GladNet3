using GladNet.Engine.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;

namespace GladNet.Lidgren.Server.Unity
{
	public class AUIDNetPeerServiceDecorator : IAUIDService<INetPeer>
	{
		private IAUIDService<ClientSessionServiceContext> lidgrenServiceContextAUIDService { get; }

		public AUIDNetPeerServiceDecorator(IAUIDService<ClientSessionServiceContext> auidServiceContext)
		{
			if (auidServiceContext == null)
				throw new ArgumentNullException(nameof(auidServiceContext), $"Provided {nameof(IAUIDService<ClientSessionServiceContext>)} must not be null.");

			lidgrenServiceContextAUIDService = auidServiceContext;
		}

		public INetPeer this[int key]
		{
			get
			{
				return lidgrenServiceContextAUIDService[key]?.ClientNetPeer;
			}

			set
			{
				throw new InvalidOperationException("Cannot set netpeer through netpeer decoractor.");
			}
		}

		public int Count
		{
			get
			{
				return lidgrenServiceContextAUIDService.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return lidgrenServiceContextAUIDService.IsReadOnly;
			}
		}

		public ICollection<int> Keys
		{
			get
			{
				return lidgrenServiceContextAUIDService.Keys;
			}
		}

		public ReaderWriterLockSlim syncObj
		{
			get
			{
				return lidgrenServiceContextAUIDService.syncObj;
			}
		}

		public ICollection<INetPeer> Values
		{
			get
			{
				return lidgrenServiceContextAUIDService.Values.Select(x => x.ClientNetPeer).ToList();
			}
		}

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
			if (ContainsKey(key))
				value = lidgrenServiceContextAUIDService[key]?.ClientNetPeer;
			else
				value = null;

			return value != null;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
