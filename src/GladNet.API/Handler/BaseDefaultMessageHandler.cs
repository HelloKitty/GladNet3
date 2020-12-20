using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GladNet
{
	/// <summary>
	/// Simplified type for default message handlers.
	/// These derive from <see cref="BaseSpecificMessageHandler{TMessageType,TBaseMessageType,TMessageContext}"/> but instead
	/// just having matching type args for Message and BaseMessage types.
	/// </summary>
	/// <typeparam name="TMessageType"></typeparam>
	/// <typeparam name="TMessageContext"></typeparam>
	public abstract class BaseDefaultMessageHandler<TMessageType, TMessageContext> 
		: BaseSpecificMessageHandler<TMessageType, TMessageType, TMessageContext> 
		where TMessageType : class
	{

	}
}
