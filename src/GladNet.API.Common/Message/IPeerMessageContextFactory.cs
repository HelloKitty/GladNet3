using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet
{
	/// <summary>
	/// Contract for types that provide construction functionality
	/// for <see cref="IPeerMessageContext{TPayloadBaseType}"/>.
	/// </summary>
	public interface IPeerMessageContextFactory
	{
		//TODO: Update doc
		/// <summary>
		/// Creates a new client message context based around the <see cref="client"/>
		/// and contextless relative to the actual message.
		/// </summary>
		/// <typeparam name="TPayloadBaseType">The payload basetype.</typeparam>
		/// <returns>A new message context.</returns>
		IPeerMessageContext<TPayloadBaseType> Create<TPayloadBaseType>(IConnectionService connectionService, IPeerRequestSendService<TPayloadBaseType> sendService, IPeerRequestSendService<TPayloadBaseType> requestService)
			where TPayloadBaseType : class;
	}
}
