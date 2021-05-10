using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using DistributedDocs.Server.Models;
using DistributedDocs.Server.Services;
using DistributedDocs.Server.Users;
using DistributedDocs.Utils;

namespace DistributedDocs.Server.ConnectReceiver
{
	internal sealed class ConnectReceiver : Disposable
	{
		private readonly DocumentContext _documentContext;
		private readonly IUserStorage _userStorage;

		private const int Port = 5554;
		private readonly IPAddress _group = IPAddress.Parse("224.0.0.155");
		private readonly UdpClient _udpReceiver;

		private readonly Thread _thread;

		public ConnectReceiver(DocumentContext documentContext, IUserStorage userStorage)
		{
			_documentContext = documentContext;
			_userStorage = userStorage;
			_udpReceiver = new UdpClient(Port);
			_udpReceiver.JoinMulticastGroup(_group);

			_thread = new Thread(ReceiveConnects);
			_thread.Start();
		}

		private void ReceiveConnects()
		{
			try
			{
				while (true)
				{
					IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
					var bytes = _udpReceiver.Receive(ref endpoint);

					try
					{
						var message = DocumentsInfoPackage.Deserialize(bytes);

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
						}
					}
					catch (ArgumentException)
					{

						// TODO: log
					}

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