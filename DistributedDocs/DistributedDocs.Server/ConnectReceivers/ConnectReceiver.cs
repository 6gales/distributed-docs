using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using DistributedDocs.Server.Models;
using DistributedDocs.Server.Services;
using DistributedDocs.Server.Users;
using DistributedDocs.Utils;
using Microsoft.Extensions.Logging;

namespace DistributedDocs.Server.ConnectReceivers
{
	internal sealed class ConnectReceiver : Disposable
	{
		private readonly DocumentContext _documentContext;
		private readonly IUserStorage _userStorage;
		private readonly ILogger<ConnectReceiver> _logger;

		private const int Port = 5555;
		private readonly IPAddress _group = IPAddress.Parse("224.0.0.155");
		private readonly UdpClient _udpReceiver;

		private readonly Thread _thread;

		public ConnectReceiver(DocumentContext documentContext,
			IUserStorage userStorage,
			ILogger<ConnectReceiver> logger)
		{
			_documentContext = documentContext;
			_userStorage = userStorage;
			_logger = logger;
			_udpReceiver = new UdpClient(Port) { MulticastLoopback = false };

			_udpReceiver.JoinMulticastGroup(_group);

			_thread = new Thread(ReceiveConnects);
			_thread.Start();
		}

		private void HandleMessage(byte[] messageBytes)
		{
			try
			{
				var message = DocumentsInfoPackage.Deserialize(messageBytes);

				if (message.DocumentInfos != null)
				{
					foreach (var messageDocumentInfo in message.DocumentInfos)
					{
						_documentContext.AddRemoteDocument(messageDocumentInfo.Key.DocumentId,
							messageDocumentInfo.Key);

						foreach (var user in messageDocumentInfo.Value)
						{
							_userStorage.AddUser(messageDocumentInfo.Key.DocumentId, user);
						}

					}
					_logger.Log(LogLevel.Information, 
						string.Join(" ", message.DocumentInfos.Keys.Select(a => 
							$"[docId: {a.DocumentId} | docName:{a.DocumentName}]")));
				}
			}
			catch (ArgumentException)
			{
				// TODO: log
			}
		}

		private void ReceiveConnects()
		{
			try
			{
				while (true)
				{
					IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
					var bytes = _udpReceiver.Receive(ref endpoint);

					HandleMessage(bytes);

				}
				
			}
			catch (ThreadAbortException)
			{

			}

		}

		protected override void ReleaseResources()
		{
			_thread.Abort();
			_udpReceiver.Dispose();
		}
	}
}