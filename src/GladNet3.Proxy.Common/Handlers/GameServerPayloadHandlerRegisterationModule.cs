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
	public abstract class GameServerPayloadHandlerRegisterationModule<TIncomingPayloadType, TOutgoingPayloadType> : PayloadHandlerRegisterationModule<TIncomingPayloadType, TOutgoingPayloadType, IProxiedMessageContext<TOutgoingPayloadType, TIncomingPayloadType>> 
		where TOutgoingPayloadType : class 
		where TIncomingPayloadType : class
	{
		/// <summary>
		/// The string constant used to name client handlers.
		/// </summary>
		public static string ServerHandlerNamedConstant { get; } = "Server";

		/// <inheritdoc />
		protected override IEnumerable<Type> OnProcessHandlerTypes(IEnumerable<Type> handlerTypes)
		{
			//Since game packet payloads are same for incoming and outgoing we need to
			//add the required attribute
			return base.OnProcessHandlerTypes(handlerTypes)
				.Where(t => t.GetCustomAttribute<ServerPayloadHandlerAttribute>(true) != null);
		}

		/// <inheritdoc />
		protected override void ExtendedHandlerRegisterationDetails(IRegistrationBuilder<object, SimpleActivatorData, SingleRegistrationStyle> registrationBuilder, Type handlerType)
		{
			base.ExtendedHandlerRegisterationDetails(registrationBuilder, handlerType);
			registrationBuilder.Named(ServerHandlerNamedConstant, handlerType); //we need to register this as a Server handler since the Types are the same.
		}
	}
}
