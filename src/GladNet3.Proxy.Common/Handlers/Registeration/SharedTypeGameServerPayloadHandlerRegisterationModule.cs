using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac.Builder;

namespace GladNet
{
	/// <summary>
	/// Simplied Type alias for game registeration module for server handlers.
	/// </summary>
	public abstract class SharedTypeGameServerPayloadHandlerRegisterationModule<TBasePayloadType> : PayloadHandlerRegisterationModule<TBasePayloadType, TBasePayloadType, IProxiedMessageContext<TBasePayloadType, TBasePayloadType>> 
		where TBasePayloadType : class 
	{

	}
}
