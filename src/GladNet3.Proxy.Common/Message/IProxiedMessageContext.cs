using GladNet;

namespace GladNet
{
	public interface IProxiedMessageContext<TPayloadWriteType, TPayloadReadType> : IPeerMessageContext<TPayloadWriteType> 
		where TPayloadWriteType : class
		where TPayloadReadType : class
	{
		/// <summary>
		/// The connection this message is being proxied from.
		/// (Ex. This is the connection you would forward the message to)
		/// </summary>
		IManagedNetworkClient<TPayloadReadType, TPayloadWriteType> ProxyConnection { get; }
	}
}