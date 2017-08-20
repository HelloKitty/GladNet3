using GladNet.Message;
using GladNet.Payload;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf.Meta;

namespace GladNet.Serializer.Protobuf.Tests
{
	[TestFixture]
	public class ProtobufnetCouldntDeserializeMessageWithNetSendable
	{

		//This has trouble rerunning sometimes due to sharing data between tests
		//So we can ignore it on Mono to protect Travis false positives
#if !__MonoCS__
		[Test]
		public static void Test_Probuf_Serializes_Message_And_Serializes_Message_With_Payload()
		{
			ProtobufnetRegistry registry = new ProtobufnetRegistry();

			registry.Register(typeof(NetworkMessage));
			registry.Register(typeof(RequestMessage));
			registry.Register(typeof(RequestMessage));
			registry.Register(typeof(EventMessage));
			registry.Register(typeof(ResponseMessage));
			registry.Register(typeof(StatusMessage));
			registry.Register(typeof(NetSendable<>));
			registry.Register(typeof(NetSendable<PacketPayload>));
			registry.Register(typeof(TestPayload));




			RequestMessage message = new RequestMessage(new TestPayload());
			message.Payload.Serialize(new ProtobufnetSerializerStrategy());
			message.Payload.Deserialize(new ProtobufnetDeserializerStrategy());
			Assert.NotNull(message.Payload.Data);
			message.Payload.Serialize(new ProtobufnetSerializerStrategy());

			ProtobufnetSerializerStrategy serializer = new ProtobufnetSerializerStrategy();

			Assert.NotNull((new ProtobufnetDeserializerStrategy()).Deserialize<NetworkMessage>(serializer.Serialize(message as NetworkMessage)).Payload);
		}
#endif

		[Test]
		public static void Test_Protobufnet_Can_Serialize_NetSendablePackPayload()
		{
			ProtobufnetRegistry registry = new ProtobufnetRegistry();

			registry.Register(typeof(TestSerialization));

			TestSerialization netsendable = new TestSerialization(new TestPayload());

			ProtobufnetSerializerStrategy serializer = new ProtobufnetSerializerStrategy();

			Assert.NotNull((new ProtobufnetDeserializerStrategy()).Deserialize<TestSerialization>(serializer.Serialize(netsendable)));
		}

		[GladNetSerializationContract]
		[GladNetSerializationInclude(10, typeof(PacketPayload), false)]
		public class TestPayload : PacketPayload
		{

		}

		[GladNetSerializationContract]
		public class TestSerialization
		{
			[GladNetMember(5, IsRequired = true)]
			public NetSendable<PacketPayload> netsendable { get; private set; }

			protected TestSerialization()
			{

			}

			public TestSerialization(PacketPayload payload)
			{
				netsendable = new NetSendable<PacketPayload>(payload);
			}
		}
	}
}
