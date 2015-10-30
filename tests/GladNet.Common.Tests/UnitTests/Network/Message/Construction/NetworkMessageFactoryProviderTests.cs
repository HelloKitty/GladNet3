using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GladNet.Common.Tests
{
	[TestFixture]
	public class NetworkMessageFactoryProviderTests
	{
		[Test]
		public static void Test_Construction_Of_Factory_Provider()
		{
			INetworkMessageFactoryProvider provider = new NetworkMessageFactoryProvider();
		}

		[Test]
		public static void Test_Registeration_Of_Factory()
		{
			//arrange
			NetworkMessageFactoryProvider provider = new NetworkMessageFactoryProvider();
			Mock<INetworkMessageFactory> factory = new Mock<INetworkMessageFactory>();

			//act
			provider.Register<EventMessage>(factory.Object);

			//assert
			Assert.AreEqual(factory.Object, provider.GetFactoryFor<EventMessage>());
			Assert.AreEqual(factory.Object, provider.GetFactoryFor(OperationType.Event));
		}

		[Test]
		public static void Test_Registeration_Of_Factory_With_Null([EnumRange(typeof(OperationType))] OperationType opType)
		{
			//arrange
			NetworkMessageFactoryProvider provider = new NetworkMessageFactoryProvider();

			//assert
			Assert.Throws(typeof(ArgumentNullException), () => provider.Register<EventMessage>(null));
			Assert.Throws(typeof(InvalidOperationException), () => provider.GetFactoryFor(opType));
		}

		[Test]
		public static void Test_Provided_Register_Each_Type_And_Check([EnumRange(typeof(OperationType))] OperationType opType)
		{
			//arrange
			NetworkMessageFactoryProvider provider = new NetworkMessageFactoryProvider();
			INetworkMessageFactory factory = GeneralNetworkMessageFactory.Create(p => Activator.CreateInstance(opType.ToNetworkMessageType(), new object[] { p }) as NetworkMessage);

			//act
			provider.Register(opType, factory);

			//Assert
			Assert.NotNull(provider.GetFactoryFor(opType));
			Assert.AreSame(factory, provider.GetFactoryFor(opType));
		}

		[Test]
		public static void Test_Provided_Factory_Is_Providing_Correct_Factory()
		{
			//arrange
			NetworkMessageFactoryProvider provider = new NetworkMessageFactoryProvider();
			Dictionary<OperationType, INetworkMessageFactory> factoryMap = new Dictionary<OperationType, INetworkMessageFactory>();

			factoryMap.Add(OperationType.Event, GeneralNetworkMessageFactory.Create(p => new EventMessage(p)));
			factoryMap.Add(OperationType.Request, GeneralNetworkMessageFactory.Create(p => new RequestMessage(p)));
			factoryMap.Add(OperationType.Response, GeneralNetworkMessageFactory.Create(p => new ResponseMessage(p)));

			//act
			foreach (var kp in factoryMap)
			{
				provider.Register(kp.Key, kp.Value);
			}

			//assert
			foreach (OperationType opType in Enum.GetValues(typeof(OperationType)))
			{
				Assert.NotNull(provider.GetFactoryFor(opType));
				Assert.AreSame(factoryMap[opType], provider.GetFactoryFor(opType));

				foreach (OperationType opType2 in Enum.GetValues(typeof(OperationType)))
				{
					if (opType != opType2)
						Assert.AreNotSame(factoryMap[opType2], provider.GetFactoryFor(opType));
				}
			}
		}

		[Test]
		public static void Test_Provided_Factory_Is_Providing_Correct_Factory_With_Fluent_Extension_Register()
		{
			//arrange
			NetworkMessageFactoryProvider factoryRegisterProvider = NetworkMessageFactoryProvider.Create()
				.RegisterAs(GeneralNetworkMessageFactory.Create(p => new EventMessage(p)))
				.RegisterAs(GeneralNetworkMessageFactory.Create(p => new RequestMessage(p)))
				.RegisterAs(GeneralNetworkMessageFactory.Create(p => new ResponseMessage(p)));

			//assert
			Assert.AreEqual(typeof(EventMessage), factoryRegisterProvider.GetFactoryFor<EventMessage>().With(new StatusChangePayload(NetStatus.Connected)).GetType());
			Assert.AreEqual(typeof(RequestMessage), factoryRegisterProvider.GetFactoryFor<RequestMessage>().With(new StatusChangePayload(NetStatus.Connected)).GetType());
			Assert.AreEqual(typeof(ResponseMessage), factoryRegisterProvider.GetFactoryFor<ResponseMessage>().With(new StatusChangePayload(NetStatus.Connected)).GetType());
		}

		[Test]
		public static void Test_Provided_Factory_Is_Providing_Correct_Factory_With_Fluent_Extension_By_Func_Register()
		{
			//arrange
			NetworkMessageFactoryProvider netMessageFactoryProvider = NetworkMessageFactoryProvider.Create()
				.RegisterAs(p => new EventMessage(p))
				.RegisterAs(p => new RequestMessage(p))
				.RegisterAs(p => new ResponseMessage(p));

			//assert
			Assert.AreEqual(typeof(EventMessage), netMessageFactoryProvider.GetFactoryFor<EventMessage>().With(new StatusChangePayload(NetStatus.Connected)).GetType());
			Assert.AreEqual(typeof(RequestMessage), netMessageFactoryProvider.GetFactoryFor<RequestMessage>().With(new StatusChangePayload(NetStatus.Connected)).GetType());
			Assert.AreEqual(typeof(ResponseMessage), netMessageFactoryProvider.GetFactoryFor<ResponseMessage>().With(new StatusChangePayload(NetStatus.Connected)).GetType());
		}

		[Test]
		public static void Test_StatusMessageFactory_Registeration()
		{
			//arrange and add
			StatusMessageFactory factory = new StatusMessageFactory();

			NetworkMessageFactoryProvider provider = NetworkMessageFactoryProvider.Create()
				.RegisterAs(factory);

			//assert
			Assert.IsNotNull(provider.GetFactoryFor<StatusMessage>());
			Assert.AreEqual(factory, provider.GetFactoryFor<StatusMessage>());
		}
	}
}
