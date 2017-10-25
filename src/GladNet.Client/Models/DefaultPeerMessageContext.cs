using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet
{
	/// <summary>
	/// Default client message context.
	/// Implements <see cref="IPeerMessageContext{TPayloadBaseType}"/>.
	/// </summary>
	/// <typeparam name="TPayloadBaseType">The type of the base payload.</typeparam>
	public sealed class DefaultPeerMessageContext<TPayloadBaseType> : IPeerMessageContext<TPayloadBaseType>
		where TPayloadBaseType : class
	{
		/// <inheritdoc />
		public IConnectionService ConnectionService { get; }

		/// <inheritdoc />
		public IPeerPayloadSendService<TPayloadBaseType> PayloadSendService { get; }

		/// <inheritdoc />
		public IPeerRequestSendService<TPayloadBaseType> RequestSendService { get; }

		/// <inheritdoc />
		public DefaultPeerMessageContext(IConnectionService connectionService, IPeerPayloadSendService<TPayloadBaseType> payloadSendService, IPeerRequestSendService<TPayloadBaseType> requestSendService)
		{
			if(connectionService == null) throw new ArgumentNullException(nameof(connectionService));
			if(payloadSendService == null) throw new ArgumentNullException(nameof(payloadSendService));
			if(requestSendService == null) throw new ArgumentNullException(nameof(requestSendService));

			ConnectionService = connectionService;
			PayloadSendService = payloadSendService;
			RequestSendService = requestSendService;
		}
	}
}
