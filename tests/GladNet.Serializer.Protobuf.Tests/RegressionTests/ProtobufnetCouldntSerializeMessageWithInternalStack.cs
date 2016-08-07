using GladNet.Message;
using GladNet.Payload;
using Moq;
using NUnit.Framework;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GladNet.Serializer.Protobuf.Tests
{
	[TestFixture]
	public static class ProtobufnetCouldntSerializeMessageWithInternalStack
	{
		[Test]
		public static void Test_Can_Serialize_MessageType()
		{
			ProtobufnetRegistry registry = new ProtobufnetRegistry();

			registry.Register(typeof(NetworkMessage));
			registry.Register(typeof(RequestMessage));

			RequestMessage message = new RequestMessage(Mock.Of<PacketPayload>());

			ProtobufnetSerializerStrategy serializer = new ProtobufnetSerializerStrategy();

			Assert.NotNull(serializer.Serialize(message));
		}

		[Test]
		public static void Test_Can_Deserialize_MessageType()
		{
			ProtobufnetRegistry registry = new ProtobufnetRegistry();

			registry.Register(typeof(NetworkMessage));
			registry.Register(typeof(RequestMessage));
			registry.Register(typeof(EventMessage));
			registry.Register(typeof(ResponseMessage));
			

			RequestMessage message = new RequestMessage(new StatusChangePayload(Common.NetStatus.Connected));

			ProtobufnetSerializerStrategy serializer = new ProtobufnetSerializerStrategy();

			Assert.NotNull((new ProtobufnetDeserializerStrategy()).Deserialize<RequestMessage>(serializer.Serialize(message as NetworkMessage)));
		}

		[Test]
		public static void Test_Serialized_Stack_Results_In_Same_Order()
		{
			ProtobufnetRegistry registry = new ProtobufnetRegistry();

			registry.Register(typeof(NetworkMessage));
			registry.Register(typeof(RequestMessage));
			registry.Register(typeof(EventMessage));
			registry.Register(typeof(ResponseMessage));


			RequestMessage message = new RequestMessage(new StatusChangePayload(Common.NetStatus.Connected));
			message.Push(1);
			message.Push(2);
			message.Push(3);
			message.Push(4);

			ProtobufnetSerializerStrategy serializer = new ProtobufnetSerializerStrategy();

			Assert.NotNull((new ProtobufnetDeserializerStrategy()).Deserialize<RequestMessage>(serializer.Serialize(message as NetworkMessage)));
			RequestMessage deserializedMessage = (new ProtobufnetDeserializerStrategy()).Deserialize<RequestMessage>(serializer.Serialize(message as NetworkMessage));

			//check that the stack is in the same order
			Assert.AreEqual(message.Pop(), deserializedMessage.Pop());
			Assert.AreEqual(message.Pop(), deserializedMessage.Pop());
			Assert.AreEqual(message.Pop(), deserializedMessage.Pop());
			Assert.AreEqual(message.Pop(), deserializedMessage.Pop());
		}

		[Test]
		public static void Test_Serialized_Stack_Empty_Results_In_Not_Throw()
		{
			ProtobufnetRegistry registry = new ProtobufnetRegistry();

			registry.Register(typeof(NetworkMessage));
			registry.Register(typeof(RequestMessage));
			registry.Register(typeof(EventMessage));
			registry.Register(typeof(ResponseMessage));


			RequestMessage message = new RequestMessage(new StatusChangePayload(Common.NetStatus.Connected));
			message.Push(1);
			message.Pop();

			ProtobufnetSerializerStrategy serializer = new ProtobufnetSerializerStrategy();

			Assert.DoesNotThrow(() => (new ProtobufnetDeserializerStrategy()).Deserialize<RequestMessage>(serializer.Serialize(message as NetworkMessage)));
		}
	}
}
