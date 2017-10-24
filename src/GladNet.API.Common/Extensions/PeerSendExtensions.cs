using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GladNet
{
	public static class PeerSendExtensions
	{
		//This extension exists mostly to support the old TCP only API
		/// <summary>
		/// Sends the provided <see cref="payload"/>
		/// </summary>
		/// <typeparam name="TPayloadType">The type of payload.</typeparam>
		/// <typeparam name="TPayloadBaseType">The base type of the payload.</typeparam>
		/// <param name="sendService">The extended send service.</param>
		/// <param name="payload">The payload to send.</param>
		/// <returns>Indicates the result of the send message operation.</returns>
		public static async Task<SendResult> SendMessage<TPayloadBaseType, TPayloadType>(this IPeerPayloadSendService<TPayloadBaseType> sendService, TPayloadType payload)
			where TPayloadType : class, TPayloadBaseType 
			where TPayloadBaseType : class
		{
			if(sendService == null) throw new ArgumentNullException(nameof(sendService));
			if(payload == null) throw new ArgumentNullException(nameof(payload));

			//We default to reliable ordered as if this is TCP
			return await sendService.SendMessage(payload, DeliveryMethod.ReliableOrdered);
		}

		//This extension mostly exists for the old TCP-only API
		/// <summary>
		/// Sends the <see cref="request"/> payload and provided a future awaitable
		/// that can yield the <typeparamref name="TResponseType"/> payload.
		/// </summary>
		/// <typeparam name="TResponseType"></typeparam>
		/// <typeparam name="TPayloadBaseType"></typeparam>
		/// <param name="sendService"></param>
		/// <param name="request">The request payload.</param>
		/// <returns>A future that contains the response.</returns>
		public static async Task<TResponseType> SendRequestAsync<TPayloadBaseType, TResponseType>(this IPeerRequestSendService<TPayloadBaseType> sendService, TPayloadBaseType request) 
			where TPayloadBaseType : class
		{
			if(sendService == null) throw new ArgumentNullException(nameof(sendService));
			if(request == null) throw new ArgumentNullException(nameof(request));

			//Since no delivry or cancel was provided we should default to reliable and also make sure there is no cancel
			return await sendService.SendRequestAsync<TResponseType>(request, DeliveryMethod.ReliableOrdered, CancellationToken.None);
		}
	}
}
