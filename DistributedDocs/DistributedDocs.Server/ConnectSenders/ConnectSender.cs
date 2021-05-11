using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using DistributedDocs.Server.ClientModels;
using DistributedDocs.Server.Models;
using DistributedDocs.Server.Services;
using DistributedDocs.Server.Users;
using DistributedDocs.Utils;
using Microsoft.Extensions.Logging;

namespace DistributedDocs.Server.ConnectSenders
{
	internal sealed class ConnectSender : Disposable
	{
		private readonly IUserStorage _userStorage;
		private readonly DocumentContext _documentContext;
		private readonly ILogger<ConnectSender> _logger;

		private readonly Timer _timer;
		private readonly UdpClient _udpSender;
		private const int Port = 5555;
		private readonly IPAddress _group = IPAddress.Parse("224.0.0.155");
		private readonly IPEndPoint _localEp = new IPEndPoint(IPAddress.Any, Port);
		private readonly IPEndPoint _groupEndPoint;

		public ConnectSender(IUserStorage userStorage, DocumentContext documentContext, ILogger<ConnectSender> logger)
		{
			_userStorage = userStorage;
			_documentContext = documentContext;
			_logger = logger;

			_groupEndPoint = new IPEndPoint(_group, Port);
			_udpSender = new UdpClient();
			_udpSender.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress,
				true);
			//_udpSender.Client.Bind(_localEp);
			_udpSender.JoinMulticastGroup(_group);

			_timer = new Timer(Send, null, 0, 1000);
		}

		private DocumentsInfoPackage CreateMessage()
		{
			var docs = _documentContext.GetDocuments();
			Dictionary<DocumentInfo, List<User>> docsUsers = 
				docs.ToDictionary(d => d,
					d=> _userStorage.GetUserList(d.DocumentId)
						.Cast<User>()
						.ToList());

			var documentsInfoPackage = new DocumentsInfoPackage
			{
				DocumentInfos = docsUsers,
			};

			return documentsInfoPackage;
		}

		private bool CreateSendMessage(out byte[] messageBytes)
		{
			var message = CreateMessage();
			if (message.DocumentInfos?.Any() != true)
			{
				messageBytes = Array.Empty<byte>();
				return false;
			}

			messageBytes = message.Serialize();
			_logger.Log(LogLevel.Information,
				string.Join(" ", message.DocumentInfos.Keys.Select(a =>
					$"[docId: {a.DocumentId} | docName:{a.DocumentName}]")));
			return true;
		}

		private void Send(object? state)
		{
			try
			{
				if (CreateSendMessage(out var message))
				{
					_udpSender.SendAsync(message, message.Length, _groupEndPoint);
				}

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