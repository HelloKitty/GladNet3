using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GladNet;

namespace GladNet
{
	/// <summary>
	/// Default implementation of the <see cref="IPeerMessageContextFactory"/>.
	/// </summary>
	public sealed class DefaultMessageContextFactory : IPeerMessageContextFactory
	{
		/// <inheritdoc />
		public IPeerMessageContext<TPayloadBaseType> Create<TPayloadBaseType>(IConnectionService connectionService, IPeerPayloadSendService<TPayloadBaseType> sendService, IPeerRequestSendService<TPayloadBaseType> requestService)
			where TPayloadBaseType : class
		{
			if(connectionService == null) throw new ArgumentNullException(nameof(connectionService));
			if(sendService == null) throw new ArgumentNullException(nameof(sendService));

			//This doesn't have to work like this, it could be other services/dependencies,
			//but the only implementation right now is the client itself.
			return new DefaultPeerMessageContext<TPayloadBaseType>(connectionService, sendService, requestService);
		}
	}
}
