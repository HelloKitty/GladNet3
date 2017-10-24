using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GladNet
{
	public static class IPeerPayloadSendServiceExtensions
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
	}
}
