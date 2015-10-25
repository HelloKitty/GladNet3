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
			Assert.AreEqual(factory.Object, provider.CreateType<EventMessage>());
			Assert.AreEqual(factory.Object, provider.CreateType(OperationType.Event));
		}

		[Test]
		public static void Test_Registeration_Of_Factory_With_Null([EnumRange(typeof(OperationType))] OperationType opType)
		{
			//arrange
			NetworkMessageFactoryProvider provider = new NetworkMessageFactoryProvider();

			//assert
			Assert.Throws(typeof(ArgumentNullException), () => provider.Register<EventMessage>(null));
			Assert.Throws(typeof(InvalidOperationException), () => provider.CreateType(opType));
		}

		[Test]
		public static void Test_Provided_Register_Each_Type_And_Check([EnumRange(typeof(OperationType))] OperationType opType)
		{
			//arrange
			NetworkMessageFactoryProvider provider = new NetworkMessageFactoryProvider();
			INetworkMessageFactory factory = 
				new GeneralNetworkMessageFactory(p => Activator.CreateInstance(opType.ToNetworkMessageType(), new object[] { p }) as NetworkMessage);

			//act
			provider.Register(opType, factory);

			//Assert
			Assert.NotNull(provider.CreateType(opType));
			Assert.AreSame(factory, provider.CreateType(opType));
		}

		[Test]
		public static void Test_Provided_Factory_Is_Providing_Correct_Factory()
		{
			//arrange
			NetworkMessageFactoryProvider provider = new NetworkMessageFactoryProvider();
			Dictionary<OperationType, INetworkMessageFactory> factoryMap = new Dictionary<OperationType, INetworkMessageFactory>();

			factoryMap.Add(OperationType.Event, new GeneralNetworkMessageFactory(p => new EventMessage(p)));
			factoryMap.Add(OperationType.Request, new GeneralNetworkMessageFactory(p => new RequestMessage(p)));
			factoryMap.Add(OperationType.Response, new GeneralNetworkMessageFactory(p => new ResponseMessage(p)));

			//act
			foreach (var kp in factoryMap)
			{
				provider.Register(kp.Key, kp.Value);
			}

			//assert
			foreach (OperationType opType in Enum.GetValues(typeof(OperationType)))
			{
				Assert.NotNull(provider.CreateType(opType));
				Assert.AreSame(factoryMap[opType], provider.CreateType(opType));

				foreach (OperationType opType2 in Enum.GetValues(typeof(OperationType)))
				{
					if (opType != opType2)
						Assert.AreNotSame(factoryMap[opType2], provider.CreateType(opType));
				}
			}
		}
	}
}
