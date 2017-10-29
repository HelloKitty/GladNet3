using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GladNet
{
	/// <summary>
	/// Service that can generate awaitable responses
	/// for recieveing a payload of a specific type.
	/// </summary>
	public sealed class PayloadInterceptMessageSendService<TPayloadBaseType> : IPeerRequestSendService<TPayloadBaseType>
		where TPayloadBaseType : class
	{
		/// <summary>
		/// Service that allows for interception of payloads.
		/// </summary>
		private IPayloadInterceptable InterceptionService { get; }

		/// <summary>
		/// The client send service.
		/// </summary>
		private IPeerPayloadSendService<TPayloadBaseType> SendService { get; }

		/// <inheritdoc />
		public PayloadInterceptMessageSendService(IPayloadInterceptable interceptionService, IPeerPayloadSendService<TPayloadBaseType> sendService)
		{
			if(interceptionService == null) throw new ArgumentNullException(nameof(interceptionService));
			if(sendService == null) throw new ArgumentNullException(nameof(sendService));

			InterceptionService = interceptionService;
			SendService = sendService;
		}

		/// <inheritdoc />
		public async Task<TResponseType> SendRequestAsync<TResponseType>(TPayloadBaseType request, DeliveryMethod method, CancellationToken cancellationToken)
		{
			//TODO: There is a design race condition here. No matter the order.
			//We opt for this particular race condition because it would be better to recieve
			//responses from slightly before us sending the request than to miss them due to a race
			//before registering the interception.
			Task<TResponseType> resulTask = InterceptionService.InterceptPayload<TResponseType>(cancellationToken);

			await SendService.SendMessage(request, method)
				.ConfigureAwait(false);

			return await resulTask
				.ConfigureAwait(false);
		}
	}
}
