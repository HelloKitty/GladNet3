using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using GladNet;
using JetBrains.Annotations;

namespace GladNet
{
	public abstract class BaseGamePayloadHandler<TSpecificPayloadType, TBasePayloadType, TOutgoingPayloadType> : IPeerPayloadSpecificMessageHandler<TSpecificPayloadType, TOutgoingPayloadType, IProxiedMessageContext<TOutgoingPayloadType, TBasePayloadType>>
		where TSpecificPayloadType : class, TBasePayloadType
		where TBasePayloadType : class
		where TOutgoingPayloadType : class
	{
		/// <summary>
		/// The logger for the handler.
		/// </summary>
		protected ILog Logger { get; }

		/// <inheritdoc />
		protected BaseGamePayloadHandler([NotNull] ILog logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public abstract Task OnHandleMessage(IProxiedMessageContext<TOutgoingPayloadType, TBasePayloadType> context, TSpecificPayloadType payload);

		/// <inheritdoc />
		public async Task HandleMessage(IProxiedMessageContext<TOutgoingPayloadType, TBasePayloadType> context, TSpecificPayloadType payload)
		{
			//TODO: We can't log the opcode, should GladNet force users to expose it?
			//if(Logger.IsInfoEnabled)
			//	Logger.Info($"Server Sent: {payload.GetOperationCode()}:{((int)payload.GetOperationCode()):X}");

			try
			{
				await OnHandleMessage(context, payload)
					.ConfigureAwait(false);
			}
			catch(Exception e)
			{
				if(Logger.IsErrorEnabled)
					Logger.Error($"Encountered Error in Handler: {GetType().Name} Exception: {e.Message} \n\n Stack: {e.StackTrace}");
			}
		}
	}
}
