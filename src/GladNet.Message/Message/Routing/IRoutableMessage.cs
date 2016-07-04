using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Message
{
#if !ENDUSER
	/// <summary>
	/// Routing contract for messages who can be routed.
	/// </summary>
	public interface IRoutableMessage
	{
		/// <summary>
		/// Pushes a new routing key into the message.
		/// This key indicates where a message to this message should be routed back to.
		/// </summary>
		/// <param name="routingKey">Unique routing key.</param>
		void Push(int routingKey);

		/// <summary>
		/// Removes a routing key from the message.
		/// This key indicates where this message should be forwared to.
		/// </summary>
		/// <returns>A unique routing key.</returns>
		int? Pop();

		/// <summary>
		/// Peeks at the routing key this message would use
		/// to route. Call Pop to both Peek and Remove the key before sending.
		/// </summary>
		/// <returns>Returns the routing ID or null if there are no routing IDs.</returns>
		int? Peek();

		/// <summary>
		/// Indicates if the message has any valid keys for routing.
		/// </summary>
		bool isMessageRoutable { get; }

		/// <summary>
		/// Exports the internal routing data to the target <see cref="IRoutableMessage"/>
		/// parameter <paramref name="message"/>.
		/// </summary>
		/// <param name="message"></param>
		void ExportRoutingDataTo(IRoutableMessage message);

		//Read Route-back Outside Userspace for information on why we do this: https://github.com/HelloKitty/GladNet2.Specifications/blob/master/Routing/RoutingSpecification.md
		/// <summary>
		/// Indicates if the message is currently routing back.
		/// This can help indicate to GladNet2 internals whether we should let
		/// the message even reach userspace.
		/// </summary>
		bool isRoutingBack { get; set; }
	}
#endif
}
