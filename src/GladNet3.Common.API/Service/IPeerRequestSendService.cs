using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GladNet
{
	/// <summary>
	/// Contract for a service that is able to send payloads like <see cref="IPeerPayloadSendService{TPayloadBaseType}"/>
	/// but can also return a future that will complete when a payload of that type is recieved.
	/// Implementers should make sure to shortcircuit to prevent multiple of the same payload being handled.
	/// </summary>
	/// <typeparam name="TPayloadBaseType"></typeparam>
	public interface IPeerRequestSendService<in TPayloadBaseType>
		where TPayloadBaseType : class
	{
		/// <summary>
		/// Sends the <see cref="request"/> payload and provided a future awaitable
		/// that can yield the <typeparamref name="TResponseType"/> payload.
		/// </summary>
		/// <typeparam name="TResponseType"></typeparam>
		/// <param name="request">The request payload.</param>
		/// <param name="method">The delivery method to use for sending. (Should probably use reliable since we expect a response)</param>
		/// <param name="cancellationToken">The cancellation token to use for sending the request. (A timeout)</param>
		/// <returns>A future that contains the response.</returns>
		Task<TResponseType> SendRequestAsync<TResponseType>(TPayloadBaseType request, DeliveryMethod method = DeliveryMethod.ReliableOrdered, CancellationToken cancellationToken = default(CancellationToken));
	}
}
