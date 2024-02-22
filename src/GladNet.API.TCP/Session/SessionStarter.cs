using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;

namespace GladNet
{
	//TODO: This is unfinished, not generalized enough to use in Server side yet.
	public sealed class SessionStarter<TSessionType>
		where TSessionType : ManagedSession
	{
		private ILog Logger { get; }

		public SessionStarter(ILog logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task StartAsync(TSessionType session, CancellationToken token = default)
		{
			CancellationToken sessionCancelToken = new CancellationToken(false);
			CancellationTokenSource combinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, sessionCancelToken);

			//To avoid sharing the cancel token source and dealing with race conditions involved in it being disposed we create a seperate
			//one for each task.
			CancellationToken readCancelToken = new CancellationToken(false);
			CancellationTokenSource readCancelTokenSource = CancellationTokenSource.CreateLinkedTokenSource(combinedTokenSource.Token, readCancelToken);

			CancellationToken writeCancelToken = new CancellationToken(false);
			CancellationTokenSource writeCancelTokenSource = CancellationTokenSource.CreateLinkedTokenSource(combinedTokenSource.Token, writeCancelToken);

			Task writeTask = Task.Run(async () => await StartSessionNetworkThreadAsync(session.Details, session.StartWritingAsync(writeCancelTokenSource.Token), writeCancelTokenSource, "Write"), token);
			Task readTask = Task.Run(async () => await StartSessionNetworkThreadAsync(session.Details, session.StartListeningAsync(readCancelTokenSource.Token), readCancelTokenSource, "Read"), token);

			await Task.Run(async () =>
			{
				try
				{
					await Task.WhenAny(readTask, writeTask);

					//If ANY read or write task finishes then the network should stop reading
					//by canceling the session cancel token we should cancel any remaining network task
					combinedTokenSource.Cancel();

					//Now we should wait until both tasks have finished completely, after canceling
					await Task.WhenAll(readTask, writeTask);
				}
				catch(Exception e)
				{
					//Suppress this exception, we have critical deconstruction code to run.
					if(Logger.IsErrorEnabled)
						Logger.Error($"Session: {session.Details.ConnectionId} encountered critical failure in awaiting network task. Error: {e}");
				}
				finally
				{
					if(Logger.IsDebugEnabled)
						Logger.Debug($"Session Stopping. Read State Error: {readTask.IsFaulted} Write State Error: {writeTask.IsFaulted} Read Completed: {readTask.IsCompleted} Write Completed: {writeTask.IsCompleted} Read Cancelled: {readTask.IsCanceled} Write Cancelled: {writeTask.IsCanceled}");

					if (readTask.IsFaulted)
						if(Logger.IsDebugEnabled)
							Logger.Debug($"Read Fault: {readTask.Exception}");

					if(writeTask.IsFaulted)
						if(Logger.IsDebugEnabled)
							Logger.Debug($"Write Fault: {readTask.Exception}");

					if(!combinedTokenSource.IsCancellationRequested)
						combinedTokenSource.Cancel();

					combinedTokenSource.Dispose();

					//Also all read/write tasks should be cancelled and disposed of by this point.
					//BUT we cannot know if they're cancelled, so dispose them here instead
					writeCancelTokenSource.Dispose();
					readCancelTokenSource.Dispose();
				}

				if(Logger.IsDebugEnabled)
					Logger.Debug($"Session: {session.Details.ConnectionId} Stopped Network Read/Write.");

				try
				{
					//TODO: Maybe a general disconnection event???
				}
				catch(Exception e)
				{
					if(Logger.IsErrorEnabled)
						Logger.Error($"Failed Session: {session.Details.ConnectionId} ended event. Reason: {e}");
				}
				finally
				{
					try
					{
						session.Dispose();
					}
					catch(Exception e)
					{
						if(Logger.IsErrorEnabled)
							Logger.Error($"Encountered error in Client: {session.Details.ConnectionId} session disposal. Error: {e}");
						throw;
					}
				}
			}, token);
		}

		public void Start(TSessionType session, CancellationToken token = default)
		{
			//Don't block/await (fire and forget)
#pragma warning disable 4014
			StartAsync(session, token);
#pragma warning restore 4014
		}

		private async Task StartSessionNetworkThreadAsync(SessionDetails details, Task task, CancellationTokenSource combinedTokenSource, string taskName)
		{
			if(details == null) throw new ArgumentNullException(nameof(details));
			if(task == null) throw new ArgumentNullException(nameof(task));

			try
			{
				await task;
			}
			catch(Exception e)
			{
				if(Logger.IsErrorEnabled)
					Logger.Error($"Session: {details.ConnectionId} encountered error in network {taskName} thread. Error: {e}");
			}
			finally
			{
				//It's important that if we arrive at this point WITHOUT canceling somehow
				//then we should cancel the combined source. Otherwise anything else depending on this
				//token won't actually cancel and it likely SHOULD
				if(!combinedTokenSource.IsCancellationRequested)
					combinedTokenSource.Cancel();

				combinedTokenSource.Dispose();
			}
		}
	}
}
