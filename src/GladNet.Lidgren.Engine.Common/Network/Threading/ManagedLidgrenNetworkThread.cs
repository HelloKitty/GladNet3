using GladNet.Common;
using GladNet.Engine.Common;
using GladNet.Message;
using GladNet.Payload;
using GladNet.Serializer;
using Lidgren.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GladNet.Lidgren.Engine.Common
{
	//TODO: Refactor
	public class ManagedLidgrenNetworkThread : IDisposable, INetworkThread
	{
		/// <summary>
		/// The ordered awaitable collection of <see cref="Action"/>s that dispatch outgoing messages.
		/// </summary>
		private WaitableQueue<Action> outgoingMessageQueue { get; } = new WaitableQueue<Action>();

		/// <summary>
		/// The ordered awaitable collection of <see cref="LidgrenMessageContext"/>s.
		/// </summary>
		private readonly SemaphoreQueue<LidgrenMessageContext> incomingMessageQueue = new SemaphoreQueue<LidgrenMessageContext>();

		/// <summary>
		/// Ordered collection of incoming messages.
		/// </summary>
		public IThreadedQueue<LidgrenMessageContext> IncomingMessageQueue { get { return incomingMessageQueue; } }

		private ReaderWriterLockSlim lockObj { get; }  = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion); //Unity requires norecursion

		private List<Thread> managedNetworkThreads { get; }

		//A lot of debate in .NET/C# space about when, if ever to use volatile, but from what I gather
		//when using a polling thread it's not safe to not mark a bool flag volatile since the CLR
		//makes no promise that changes to this value, which are 100% atomic, may propagate soon or ever (can the JIT/CLR make such a mistake though?)
		//Anyway, this should likely be done but someone smarter than myself should provide input on this one day.
		private volatile bool _isAlive;

		public bool isAlive { get { return _isAlive; } }

		private ISerializerStrategy serializer { get; }

		private Action<Exception> OnException { get; }

		/// <summary>
		/// Strategy used to select the sending service for a message.
		/// </summary>
		private ISendServiceSelectionStrategy sendServiceStrategy { get; }

		private ILidgrenMessageContextFactory messageContextFactory { get; }

		public ManagedLidgrenNetworkThread(ISerializerStrategy serializerStrategy, ILidgrenMessageContextFactory lidgrenMessageContextFactory, ISendServiceSelectionStrategy sendStrategy, Action<Exception> onException = null)
		{
			if (serializerStrategy == null)
				throw new ArgumentNullException(nameof(serializerStrategy), $"Provided {nameof(ISerializerStrategy)} cannot be null.");

			if (lidgrenMessageContextFactory == null)
				throw new ArgumentNullException(nameof(lidgrenMessageContextFactory), $"Provided {nameof(ILidgrenMessageContextFactory)} cannot be null.");

			if (sendStrategy == null)
				throw new ArgumentNullException(nameof(sendStrategy), $"Provided {nameof(ISendServiceSelectionStrategy)} cannot be null.");

			OnException = onException;

			sendServiceStrategy = sendStrategy;
			messageContextFactory = lidgrenMessageContextFactory;
			serializer = serializerStrategy;

			managedNetworkThreads = new List<Thread>();
		}

		public NetSendResult EnqueueMessage(OperationType opType, PacketPayload payload, DeliveryMethod method, bool encrypt, byte channel, int connectionId)
		{
			outgoingMessageQueue.syncRoot.EnterWriteLock();
			try
			{
				//This is similar to how Photon works on the Unity client.
				//They enqueue actions
				outgoingMessageQueue.Enqueue(() =>
				{
					INetworkMessageRouterService sender = sendServiceStrategy.GetRouterService(connectionId);

					sender.TrySendMessage(opType, payload, method, encrypt, channel);
				});

				//signal any handlers that a message is in the queue
				//it is important to do this in a lock. Reset could be called after Set
				//in the other thread and we'd end up with potential unhandled messages in that race condition
				outgoingMessageQueue.QueueSemaphore.Set();
			}
			finally
			{
				outgoingMessageQueue.syncRoot.ExitWriteLock();
			}

			return NetSendResult.Queued;
		}

		private void HandleIncomingMessageThreadMethod(NetPeer peer)
		{
			try
			{
				while(_isAlive)
				{
					//Don't wait forever; if you do you could encounter a hanging thread during disconnection
					if (!peer.MessageReceivedEvent.WaitOne(500))
						continue;

					OnMessageRecievedCallback(peer);
				}				
			}
			catch(Exception e)
			{
				OnException?.Invoke(e);
			}
		}

		private void HandleOutgoingMessageActionsThreadMethod()
		{
			try
			{
				while (_isAlive)
				{
					//Don't wait forever; if you do you could encounter a hanging thread during disconnection
					if (!outgoingMessageQueue.QueueSemaphore.WaitOne(500))
						continue;

					IEnumerable<Action> dequeuedActions = null;

					outgoingMessageQueue.syncRoot.EnterWriteLock();
					try
					{
						//TODO: Single message optimizations
						dequeuedActions = outgoingMessageQueue.ToList(); //tolist is more memory efficient

						//Reset the waithandle
						//Make sure to do this in the lock to prevent race coniditons
						outgoingMessageQueue.Clear();
						outgoingMessageQueue.QueueSemaphore.Reset();
					}
					finally
					{
						outgoingMessageQueue.syncRoot.ExitWriteLock();
					}

					if (dequeuedActions == null)
						throw new InvalidOperationException($"Outgoing message queue produced a null collection of messages. Expected non-null and non-empty.");

					//Invoke all outgoing message actions
					foreach (Action outgoingMessageAction in dequeuedActions)
						outgoingMessageAction();
				}
			}
			catch(Exception e)
			{
				OnException?.Invoke(e);
			}
		}

		//Make sure this runs on the reading thread
		private void OnMessageRecievedCallback(NetPeer peer)
		{
			//Read the entire chunk of messages; not just one
			//The wait handle has been consumed so we have no choice.
			//Can't do one at a time.
			List<NetIncomingMessage> messages = new List<NetIncomingMessage>(10);

			if (peer.ReadMessages(messages) == 0)
				return;

			foreach(NetIncomingMessage message in messages)
			{
				//If the context factory cannot create a context for this
				//message type then we should not enter the lock and attempt to create a context for it.
				if (!messageContextFactory.CanCreateContext(message.MessageType))
					return;

				incomingMessageQueue.syncRoot.EnterWriteLock();
				try
				{
					//enqueues the message including the meaningful context with which it was recieved.
					incomingMessageQueue.Enqueue(messageContextFactory.CreateContext(message));

					//Signal the incoming message handling thread that there is a message to recieve.
					incomingMessageQueue.QueueSemaphore.Release(1);
				}
				finally
				{
					incomingMessageQueue.syncRoot.ExitWriteLock();
				}
			}
		}

		public void Start(NetPeer incomingPeerContext)
		{
			if (_isAlive)
				throw new InvalidOperationException("A network thread should only be started once.");

			lockObj.EnterWriteLock();
			try
			{
				_isAlive = true;
				Thread outgoingThread = new Thread(new ThreadStart(HandleOutgoingMessageActionsThreadMethod));
				outgoingThread.IsBackground = true;
				outgoingThread.Start();

				managedNetworkThreads.Add(outgoingThread);

				//Create incoming thread
				Thread incomingThread = new Thread(new ThreadStart(() => HandleIncomingMessageThreadMethod(incomingPeerContext)));
				incomingThread.IsBackground = true;
				incomingThread.Start();

				managedNetworkThreads.Add(incomingThread);
			}
			finally
			{
				lockObj.ExitWriteLock();
			}
		}

		public void Stop()
		{
			lockObj.EnterWriteLock();
			try
			{
				_isAlive = false;
			}
			finally
			{
				lockObj.ExitWriteLock();
			}
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		//Added threading locks for multi-threading disposal
		protected virtual void Dispose(bool disposing)
		{
			//These checks aren't thread safe but it's better than nothing.
			if (disposedValue)
				return;

			Stop();

			//At this point the threads should be finished and we can finish disposing
			lockObj.EnterUpgradeableReadLock();
			try
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects)
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				lockObj.EnterWriteLock();
				try
				{
					//Do not Thread.Abort. Unity will throwup
					managedNetworkThreads.Clear();

					//TODO: How can we dispose the locks?
					IncomingMessageQueue.syncRoot.EnterWriteLock();
					try
					{
						incomingMessageQueue.Clear();
					}
					finally
					{
						IncomingMessageQueue.syncRoot.ExitWriteLock();
						
						//TODO: Technically we should try to dispose the locking objects. Abit complex to coordinate that though.
					}

					outgoingMessageQueue.Clear();
				}
				finally
				{
					lockObj.ExitWriteLock();
				}

				//set disposed
				disposedValue = true;
			}
			finally
			{
				lockObj.ExitUpgradeableReadLock();
				lockObj.Dispose();
			}
			
		}

		 ~ManagedLidgrenNetworkThread()
		{
		   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		   Dispose(false);
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			GC.SuppressFinalize(this);
		}
		#endregion

	}
}
