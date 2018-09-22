using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac;
using Autofac.Builder;
using GladNet;

namespace GladNet
{
	/// <summary>
	/// Simplied Type alias for game registeration module for client handlers.
	/// </summary>
	public abstract class SharedTypeGameClientPayloadHandlerRegisterationModule<TBasePayloadType> : PayloadHandlerRegisterationModule<TBasePayloadType, TBasePayloadType, IProxiedMessageContext<TBasePayloadType, TBasePayloadType>> 
		where TBasePayloadType : class 
	{

	}
}
