using GladNet.Payload;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace GladNet.Common
{
#if !ENDUSER
	public interface INetworkMessageRouteBackService
	{
		/// <summary>
		/// Routes a <see cref="IResponseMessage"/> using the provided <see cref="IMessageParameters"/>.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		SendResult RouteResponse(IResponseMessage message, IMessageParameters parameters);

		/// <summary>
		/// Routes a <see cref="IRequestMessage"/> using the provided <see cref="IMessageParameters"/>.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		SendResult RouteRequest(IRequestMessage message, IMessageParameters parameters);

		/// <summary>
		/// Routes a request payload using the provided <paramref name="routingDetails"/>
		/// provided.
		/// </summary>
		/// <param name="payload">Payload instance to be sent in the message.</param>
		/// <param name="deliveryMethod">The deseried <see cref="DeliveryMethod"/> of the message.</param>
		/// <param name="encrypt">Indicates if the message should be encrypted.</param>
		/// <param name="channel">Indicates the channel for this message to be sent over.</param>
		/// <returns>Indication of the message send state.</returns>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		SendResult RouteRequest(PacketPayload payload, IRoutableMessage routingDetails, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0);

		/// <summary>
		/// Routes a response payload using the provided <paramref name="routingDetails"/>
		/// provided.
		/// </summary>
		/// <param name="payload">Payload instance to be sent in the message.</param>
		/// <param name="deliveryMethod">The deseried <see cref="DeliveryMethod"/> of the message.</param>
		/// <param name="encrypt">Indicates if the message should be encrypted.</param>
		/// <param name="channel">Indicates the channel for this message to be sent over.</param>
		/// <returns>Indication of the message send state.</returns>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		SendResult RouteResponse(PacketPayload payload, IRoutableMessage routingDetails, DeliveryMethod deliveryMethod, bool encrypt = false, byte channel = 0);

		/// <summary>
		/// Routes a request payload using the provided <paramref name="routingDetails"/> provided
		/// Additionally this message/payloadtype is known to have static send parameters and those will be used in transit.
		/// </summary>
		/// <typeparam name="TPacketType">Type of the packet payload.</typeparam>
		/// <param name="payload">Payload instance to be sent in the message that contains static message parameters.</param>
		/// <returns>Indication of the message send state.</returns>
		SendResult RouteRequest<TPacketType>(TPacketType payload, IRoutableMessage routingDetails)
			where TPacketType : PacketPayload, IStaticPayloadParameters;

		/// <summary>
		/// Routes a response payload using the provided <paramref name="routingDetails"/> provided
		/// Additionally this message/payloadtype is known to have static send parameters and those will be used in transit.
		/// </summary>
		/// <typeparam name="TPacketType">Type of the packet payload.</typeparam>
		/// <param name="payload">Payload instance to be sent in the message that contains static message parameters.</param>
		/// <returns>Indication of the message send state.</returns>
		SendResult RouteResponse<TPacketType>(TPacketType payload, IRoutableMessage routingDetails)
			where TPacketType : PacketPayload, IStaticPayloadParameters;
	}
#endif
}
