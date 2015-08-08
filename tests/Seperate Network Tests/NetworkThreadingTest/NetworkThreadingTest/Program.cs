using GladNet.Common;
using Moq;
using ProtoBuf;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkThreadingTest
{
	class Program
	{
		public const int COUNT = 100000;
		public const string testSerializeString = "Hello, this is a test string and it aims at making the testing data sufficiently long enough to be considered a test. This will basically be serialized and then encrypted so it needs to be long enough.";

		static void Main(string[] args)
		{
			ThreadedTestFinishedWithoutThreads();
			ThreadedTest();
			UnThreadedTest();
			Console.ReadKey();
		}

		private static void ThreadedTest()
		{
			bool testing = true;
			//ConcurrentQueue<MessagePass>
			ConcurrentQueue<NetworkMessage> messages = new ConcurrentQueue<NetworkMessage>();
			ConcurrentQueue<MessagePass> messagePasses = new ConcurrentQueue<MessagePass>();

			BlockingCollection<NetworkMessage> blockingMessageQueue = new BlockingCollection<NetworkMessage>(messages);
			BlockingCollection<MessagePass> blockingMessagePassQueue = new BlockingCollection<MessagePass>(messagePasses, COUNT);
			//Add 1 million messages.
			NetworkMessage testMessage = new Mock<NetworkMessage>(Mock.Of<PacketPayload>()).Object;
			for (int i = 0; i < COUNT; i++)
				blockingMessageQueue.Add(testMessage);

			IEncryptor encryptor = Mock.Of<IEncryptor>();

			//Start a new task to content the Queue.
			Task.Factory.StartNew(() =>
				{
					while (testing)
					{
						MessagePass pass = null;
						if (blockingMessagePassQueue.TryTake(out pass, 10))
						{
							MessageContentsHandling();
						}
						else
							Thread.Sleep(1);

						if (blockingMessagePassQueue.Count == 0 && blockingMessageQueue.Count == 0)
							testing = false;
					}

				}, TaskCreationOptions.LongRunning);

			GC.Collect();
			Stopwatch timer = new Stopwatch();
			timer.Start();
			NetworkMessage message = blockingMessageQueue.Take();
			while(blockingMessageQueue.Count > 0)
			{		
				MessagePass pass = new MessagePass() { encryptor = encryptor, payload = message.Payload };
				blockingMessagePassQueue.Add(pass);

				message = blockingMessageQueue.Take();
			}

			//Wait for the other thread to finish
			while (testing) ;
			timer.Stop();

			GC.Collect();

			Console.WriteLine("Threaded: " + timer.ElapsedMilliseconds);
		}

		private static void ThreadedTestFinishedWithoutThreads()
		{
			bool testing = true;
			//ConcurrentQueue<MessagePass>
			ConcurrentQueue<NetworkMessage> messages = new ConcurrentQueue<NetworkMessage>();
			ConcurrentQueue<MessagePass> messagePasses = new ConcurrentQueue<MessagePass>();

			BlockingCollection<NetworkMessage> blockingMessageQueue = new BlockingCollection<NetworkMessage>(messages);
			BlockingCollection<MessagePass> blockingMessagePassQueue = new BlockingCollection<MessagePass>(messagePasses, COUNT);
			//Add 1 million messages.
			NetworkMessage testMessage = new Mock<NetworkMessage>(Mock.Of<PacketPayload>()).Object;
			for (int i = 0; i < COUNT; i++)
				blockingMessageQueue.Add(testMessage);

			IEncryptor encryptor = Mock.Of<IEncryptor>();

			//Start a new task to content the Queue.
			Task.Factory.StartNew(() =>
			{
				while (testing)
				{
					MessagePass pass = null;
					if (blockingMessagePassQueue.TryTake(out pass, 80))
					{
						MessageContentsHandling();
					}
					else
						Thread.Sleep(1);

					if (blockingMessagePassQueue.Count == 0 && blockingMessageQueue.Count == 0)
						testing = false;
				}

			}, TaskCreationOptions.LongRunning);

			GC.Collect();
			Stopwatch timer = new Stopwatch();
			timer.Start();
			NetworkMessage message = blockingMessageQueue.Take();
			while (blockingMessageQueue.Count > 0)
			{
				MessagePass pass = new MessagePass() { encryptor = encryptor, payload = message.Payload };
				blockingMessagePassQueue.Add(pass);

				message = blockingMessageQueue.Take();
			}
			//Wait for the other thread to finish
			testing = false;
			timer.Stop();

			GC.Collect();

			Console.WriteLine("Threaded (without waiting for threads): " + timer.ElapsedMilliseconds);
		}

		private static void UnThreadedTest()
		{
			//ConcurrentQueue<MessagePass>
			ConcurrentQueue<NetworkMessage> messages = new ConcurrentQueue<NetworkMessage>();
			ConcurrentQueue<MessagePass> messagePasses = new ConcurrentQueue<MessagePass>();

			BlockingCollection<NetworkMessage> blockingMessageQueue = new BlockingCollection<NetworkMessage>(messages);
			BlockingCollection<MessagePass> blockingMessagePassQueue = new BlockingCollection<MessagePass>(messagePasses, COUNT);
			//Add 1 million messages.
			NetworkMessage testMessage = new Mock<NetworkMessage>(Mock.Of<PacketPayload>()).Object;
			for (int i = 0; i < COUNT; i++)
				blockingMessageQueue.Add(testMessage);

			IEncryptor encryptor = Mock.Of<IEncryptor>();

			GC.Collect();
			Stopwatch timer = new Stopwatch();
			timer.Start();
			NetworkMessage message = blockingMessageQueue.Take();
			while (blockingMessageQueue.Count > 0)
			{
				MessageContentsHandling();

				message = blockingMessageQueue.Take();
			}
			timer.Stop();
			GC.Collect();

			Console.WriteLine("UnThreaded: " + timer.ElapsedMilliseconds);
		}

		private static void MessageContentsHandling()
		{
			using (MemoryStream stream = new MemoryStream())
			{
				Serializer.Serialize(stream, testSerializeString);

				using(AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
				{
					ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

					using (MemoryStream msEncrypt = new MemoryStream())
					{
						using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
						{
							using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
							{
								swEncrypt.Write(stream.ToArray());
							}
						}
					}
				}				
			}
		}
	}
}
