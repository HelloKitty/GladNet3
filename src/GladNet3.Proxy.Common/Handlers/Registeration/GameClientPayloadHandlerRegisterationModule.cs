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
	public abstract class GameClientPayloadHandlerRegisterationModule<TIncomingPayloadType, TOutgoingPayloadType> : PayloadHandlerRegisterationModule<TIncomingPayloadType, TOutgoingPayloadType, IProxiedMessageContext<TOutgoingPayloadType, TIncomingPayloadType>> 
		where TIncomingPayloadType : class 
		where TOutgoingPayloadType : class
	{
		/// <summary>
		/// The string constant used to name client handlers.
		/// </summary>
		public static string ClientHandlerNamedConstant { get; } = "Client";

		/// <inheritdoc />
		protected override IEnumerable<Type> OnProcessHandlerTypes(IEnumerable<Type> handlerTypes)
		{
			//Since game packet payloads are same for incoming and outgoing we need to
			//add the required attribute
			return base.OnProcessHandlerTypes(handlerTypes)
				.Where(t => t.GetCustomAttribute<ClientPayloadHandlerAttribute>(true) != null);
		}

		/// <inheritdoc />
		protected override void ExtendedHandlerRegisterationDetails(IRegistrationBuilder<object, SimpleActivatorData, SingleRegistrationStyle> registrationBuilder, Type handlerType)
		{
			base.ExtendedHandlerRegisterationDetails(registrationBuilder, handlerType);
			registrationBuilder.Named(ClientHandlerNamedConstant, handlerType); //we need to register this as a Server handler since the Types are the same.
		}
	}
}
