using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GladNet;

namespace GladNet
{
	/// <summary>
	/// Server version of the <see cref="IManagedNetworkClient{TPayloadWriteType,TPayloadReadType}"/>.
	/// </summary>
	/// <typeparam name="TPayloadWriteType"></typeparam>
	/// <typeparam name="TPayloadReadType"></typeparam>
	public interface IManagedNetworkServerClient<TPayloadWriteType, TPayloadReadType> : IManagedNetworkClient<TPayloadWriteType, TPayloadReadType> 
		where TPayloadWriteType : class 
		where TPayloadReadType : class
	{
		//TODO: Add server client specific stuff
		//Probably like events or delegates to subscribe to for information
	}
}
