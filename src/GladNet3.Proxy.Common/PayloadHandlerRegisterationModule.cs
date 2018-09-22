using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac;
using Autofac.Builder;
using GladNet;
using JetBrains.Annotations;
using Module = Autofac.Module;

namespace GladNet
{
	/// <summary>
	/// Base class for all handler registerations.
	/// Implementers should inherit from this Type which allows the child type to be used as a handler registeration module
	/// that can register all handlers defined in the assembly.
	/// </summary>
	/// <typeparam name="TIncomingPayloadType"></typeparam>
	/// <typeparam name="TOutgoingPayloadType"></typeparam>
	/// <typeparam name="TPeerContextType"></typeparam>
	public abstract class PayloadHandlerRegisterationModule<TIncomingPayloadType, TOutgoingPayloadType, TPeerContextType> : Module
		where TPeerContextType : IPeerMessageContext<TOutgoingPayloadType> 
		where TOutgoingPayloadType : class
		where TIncomingPayloadType : class
	{
		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			IEnumerable<Type> handlerTypes = LoadHandlerTypes();

			handlerTypes = OnProcessHandlerTypes(handlerTypes);

			//Registers each type.
			foreach(Type t in handlerTypes)
				builder.RegisterType(t)
					.AsSelf()
					.SingleInstance();

			foreach(Type t in handlerTypes)
			{
				Console.WriteLine($"Registering Type: {t} ");
				Type concretePayloadType = GetAllInterfacesOnType(t)
					.First(i => AssignableToHandlerType(i) || i.GetTypeInfo().IsGenericType && i.GetTypeInfo().GetGenericTypeDefinition() == typeof(IPeerPayloadSpecificMessageHandler<,,>))
					.GetGenericArguments()
					.First();

				Type tryHandlerType = typeof(TrySemanticsBasedOnTypePeerMessageHandler<,,,>)
					.MakeGenericType(typeof(TIncomingPayloadType), typeof(TOutgoingPayloadType), concretePayloadType, typeof(TPeerContextType));

				RegisterHandler(builder, t, tryHandlerType);
			}
		}

		protected virtual IEnumerable<Type> OnProcessHandlerTypes(IEnumerable<Type> handlerTypes)
		{
			return handlerTypes;
		}

		protected virtual Autofac.Builder.IRegistrationBuilder<object, Autofac.Builder.SimpleActivatorData, Autofac.Builder.SingleRegistrationStyle> RegisterHandler(ContainerBuilder builder, Type t, Type tryHandlerType)
		{
			var registrationBuilder = builder.Register(context =>
				{
					object handler = context.Resolve(t);

					return Activator.CreateInstance(tryHandlerType, handler);
				})
				.As(typeof(IPeerMessageHandler<TIncomingPayloadType, TOutgoingPayloadType, TPeerContextType>))
				.SingleInstance();

			ExtendedHandlerRegisterationDetails(registrationBuilder, typeof(IPeerMessageHandler<TIncomingPayloadType, TOutgoingPayloadType, TPeerContextType>));

			return registrationBuilder;
		}

		protected virtual void ExtendedHandlerRegisterationDetails(IRegistrationBuilder<object, SimpleActivatorData, SingleRegistrationStyle> registrationBuilder, Type handlerType)
		{

		}

		private IReadOnlyCollection<Type> LoadHandlerTypes()
		{
			//This loads all the handlers on the current assembly that the child Type is defined within.
			return GetType().GetTypeInfo()
				.Assembly
				.GetTypes()
				.Where(t => IsHandlerTypePredicate(t)) //must check context type now
				.Distinct()
				.ToArray();
		}

		private static bool IsHandlerTypePredicate(Type t)
		{
			bool isAssignableToHandlerType = AssignableToHandlerType(t);

			return isAssignableToHandlerType || GetAllInterfacesOnType(t).Any(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IPeerPayloadSpecificMessageHandler<,,>) && i.GenericTypeArguments.Contains(typeof(TPeerContextType)));
		}

		private static bool AssignableToHandlerType(Type t)
		{
			return typeof(IPeerPayloadSpecificMessageHandler<TIncomingPayloadType, TOutgoingPayloadType, TPeerContextType>).IsAssignableFrom(t);
		}

		public static IEnumerable<Type> GetAllInterfacesOnType(Type t)
		{
			Type temp = t;
			List<Type> interfaceTypes = new List<Type>();

			while(temp != null && temp != typeof(System.Object) && temp != typeof(ValueType))
			{
				foreach(Type interfaceType in temp.GetInterfaces())
					yield return interfaceType;

				temp = temp.BaseType;
			}
		}
	}
}
