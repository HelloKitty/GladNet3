using Booma.Common.ServerSelection;
using Booma.Payloads.ServerSelection;
using GladNet.Message;
using GladNet.Payload;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace GladNet.Serializer.Protobuf.Tests
{
	[TestFixture]
	public class CollectionSerializationRegressionTest
	{
		[Test]
		public void Test_Can_Serialize_Payloads_With_Collections()
		{
			ProtobufnetSerializerStrategy serializer = new ProtobufnetSerializerStrategy();
			ProtobufnetDeserializerStrategy deserializer = new ProtobufnetDeserializerStrategy();

			ProtobufnetRegistry registry = new ProtobufnetRegistry();

			registry.Register(typeof(NetworkMessage));
			registry.Register(typeof(ResponseMessage));
			registry.Register(typeof(GameServerListResponsePayload));

			GameServerListResponsePayload payload = new GameServerListResponsePayload(GameServerListResponseCode.Success, new List<SimpleGameServerDetailsModel>(8) { new SimpleGameServerDetailsModel("Hello", IPAddress.Any, 5, ServerRegion.EU) });
			ResponseMessage rMessage = new ResponseMessage(payload);
			rMessage.Payload.Serialize(serializer);

			ResponseMessage rMessageDeserializer = deserializer.Deserialize<NetworkMessage>(serializer.Serialize(rMessage as NetworkMessage)) as ResponseMessage;

			rMessageDeserializer.Payload.Deserialize(deserializer);

			GameServerListResponsePayload dPayload = rMessageDeserializer.Payload.Data as GameServerListResponsePayload;

			Assert.AreEqual(1, dPayload.GameServerDetails.Count());
			Assert.AreEqual("Hello", dPayload.GameServerDetails.FirstOrDefault().Name);
		}

		[Test]
		public void Test_Can_Serialize_Collections_With_Multiple_Inheritance_Type()
		{
			ProtobufnetSerializerStrategy serializer = new ProtobufnetSerializerStrategy();
			ProtobufnetDeserializerStrategy deserializer = new ProtobufnetDeserializerStrategy();

			ProtobufnetRegistry registry = new ProtobufnetRegistry();

			registry.Register(typeof(TestClassBase));
			registry.Register(typeof(TestClassChild1));
			registry.Register(typeof(TestClassChild2));

			TestClassBase testObject = new TestClassChild2() { i = 5, j = 6, k = new int[] { 1, 2, 3, 4 } };

			TestClassChild2 child = deserializer.Deserialize<TestClassBase>(serializer.Serialize(testObject)) as TestClassChild2;

			Assert.AreEqual(5, child.i);
			Assert.AreEqual(6, child.j);
			Assert.AreEqual(4, child.k.Count());

			for (int i = 0; i < child.k.Count(); i++)
				Assert.AreEqual(i + 1, child.k[i]);
		}

		[GladNetSerializationContract]
		[GladNetSerializationInclude(GladNetIncludeIndex.Index1, typeof(TestClassChild1), true)]
		public abstract class TestClassBase
		{
			[GladNetMember(GladNetDataIndex.Index1)]
			public int i;
		}

		[GladNetSerializationContract]
		[GladNetSerializationInclude(GladNetIncludeIndex.Index1, typeof(TestClassChild2), true)]
		public class TestClassChild1 : TestClassBase
		{
			[GladNetMember(GladNetDataIndex.Index1)]
			public int j;
		}

		[GladNetSerializationContract]
		public class TestClassChild2 : TestClassChild1
		{
			[GladNetMember(GladNetDataIndex.Index1)]
			public int[] k;
		}

		/*public enum GameServerListResponseCode
		{
			One = 1 << 0,
			Two = 1 << 1,
			Three = 1 << 2,
			Four = 1 << 3
		}

		public enum ServerRegion
		{
			US,
			EU,
			JP,
			CN
		}

		public interface IResponseStatus<T>
		{
			GameServerListResponseCode ResponseCode { get; }
		}

		[GladNetSerializationContract]
		[GladNetSerializationInclude(GladNetIncludeIndex.Index19, typeof(PacketPayload), false)]
		public class GameServerListResponsePayload : PacketPayload, IResponseStatus<GameServerListResponseCode>
		{
			/// <summary>
			/// Indicates the status of the response.
			/// </summary>
			[GladNetMember(GladNetDataIndex.Index1)]
			public GameServerListResponseCode ResponseCode { get; private set; }

			//private and hidden. For serializer.
			[GladNetMember(GladNetDataIndex.Index2)]
			private SimpleGameServerDetailsModel[] gameServerDetails;

			/// <summary>
			/// Collection of available gameservers.
			/// </summary>
			public IEnumerable<SimpleGameServerDetailsModel> GameServerDetails { get { return gameServerDetails; } }

			/// <summary>
			/// Creates a new gameserver list response with only a response code.
			/// </summary>
			/// <param name="code">response code.</param>
			public GameServerListResponsePayload(GameServerListResponseCode code)
			{
				ResponseCode = code;
			}

			/// <summary>
			/// Creates a new gameserver list response with the server list.
			/// </summary>
			/// <param name="code">Response code.</param>
			/// <param name="details">Details collection.</param>
			public GameServerListResponsePayload(GameServerListResponseCode code, IEnumerable<SimpleGameServerDetailsModel> details)
			{
				ResponseCode = code;

				//set to internal field
				gameServerDetails = details.ToArray();
			}

			/// <summary>
			/// Creates a new payload for the <see cref="BoomaPayloadMessageType.GetGameServerListResponse"/> packet.
			/// </summary>
			public GameServerListResponsePayload()
			{

			}
		}

		/// <summary>
		/// Wire-ready model of the gameserver details.
		/// </summary>
		[GladNetSerializationContract]
		public class SimpleGameServerDetailsModel
		{
			//Basically a subset of the DB model
			/// <summary>
			/// Name of the server (Ex. Vegas)
			/// </summary>
			[GladNetMember(GladNetDataIndex.Index1)]
			public string Name { get; private set; }

			/// <summary>
			/// Serializable byte array representing the servers's <see cref="IPAddress"/>.
			/// </summary>
			[GladNetMember(GladNetDataIndex.Index2)]
			private byte[] serverIPBytes;

			private IPAddress _ServerIP;
			/// <summary>
			/// Represents the IP address of the server.
			/// </summary>
			public IPAddress ServerIP { get { return _ServerIP == null ? _ServerIP = new IPAddress(serverIPBytes) : _ServerIP; } }

			/// <summary>
			/// Port incoming client connections should be on.
			/// </summary>
			[GladNetMember(GladNetDataIndex.Index3)]
			public int ServerPort { get; private set; }

			/// <summary>
			/// Region of the game server.
			/// </summary>
			[GladNetMember(GladNetDataIndex.Index4)]
			public ServerRegion Region { get; private set; }

			public SimpleGameServerDetailsModel(string name, IPAddress address, int port, ServerRegion region)
			{
				Name = name;
				serverIPBytes = address.GetAddressBytes();
				ServerPort = port;
				Region = region;
			}

			/// <summary>
			/// Serializer ctor
			/// </summary>
			protected SimpleGameServerDetailsModel()
			{

			}
		}*/
	}
}
