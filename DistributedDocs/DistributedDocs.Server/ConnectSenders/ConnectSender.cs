using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using DistributedDocs.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace DistributedDocs.Server.ConnectSenders
{
	internal sealed class ConnectSender : Disposable
	{
		private readonly Timer _timer;
		private readonly UdpClient _udpSender;
		private const int Port = 5555;
		private readonly IPAddress _group = IPAddress.Parse("192.168.56.1");

		public ConnectSender()
		{
			_udpSender = new UdpClient(Port);
			_udpSender.JoinMulticastGroup(_group);

			_timer = new Timer(Send, null, 0, 60 * 1000);
		}

		//public void Start() => _thread.Start();

		private byte[] CreateSendMessage()
		{
			object a = new object();


			MemoryStream ms = new MemoryStream();
			using (BsonDataWriter writer = new BsonDataWriter(ms))
			{
				JsonSerializer serializer = new JsonSerializer();
				serializer.Serialize(writer, a);
			}

			var bsonByteArray = ms.ToArray();
			return bsonByteArray;
		}

		//private 

		private void Send(object? state)
		{
			try
			{
				var message = CreateSendMessage();
				_udpSender.SendAsync(message, message.Length);
			}
			catch (ThreadAbortException)
			{
			}
		}

		protected override void ReleaseResources()
		{
			_udpSender.Dispose();
			_timer.Dispose();
		}
	}
}