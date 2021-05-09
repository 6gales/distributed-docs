﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DistributedDocs.VersionHistory;
using DistributedDocs.Server.ClientModels;
using DistributedDocs.Server.Models.ServerModels;
using DistributedDocs.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace DistributedDocs.Server.Controllers
{
	[Route("/client")]
	internal sealed class ClientSideController : ControllerBase
	{
		private readonly ServerSideCommunicator _serverSideCommunicator;
		private readonly DocumentContext _documentContext;
		private readonly IAuthorInfoEditor _authorInfoEditor;

		private List<ClientCommit> _commitsToSend = new List<ClientCommit>();
		private object _commitsSyncRoot = new object();
		private ManualResetEvent _event = new ManualResetEvent(false);


		public ClientSideController(ServerSideCommunicator serverSideCommunicator,
			DocumentContext documentContext,
			IAuthorInfoEditor authorInfoEditor)
		{
			_serverSideCommunicator = serverSideCommunicator;
			_documentContext = documentContext;
			_authorInfoEditor = authorInfoEditor;

			_documentContext.OnCommit += SendCommitToClient;
		}


		private void SendCommitToClient(ServerCommit commit)
		{
			lock (_commitsSyncRoot)
			{
				if (commit.Commit != null)
				{
					_commitsToSend.Add(commit.Commit);
					_event.Set();
				}
			}
		}

		private Task<IReadOnlyCollection<ClientCommit>> GetNewCommits()
		{
			return Task.Run(
				() =>
				{
					_event.WaitOne();

					lock (_commitsSyncRoot)
					{
						IReadOnlyCollection<ClientCommit> copy = _commitsToSend.ToList();
						_commitsToSend.Clear();
						_event.Reset();
						return copy;
					}

				}
			);
		}

		[Route("commit")]
		[HttpPost]
		public async Task<Response<EmptyResponseBody>> AddCommit([FromBody] ClientCommit clientCommit)
		{
			await _documentContext.EditDocument(clientCommit.DocumentId, clientCommit);
			return new Response<EmptyResponseBody>();
		}

		[Route("user")]
		[HttpPost]
		public Response<EmptyResponseBody> ChangeName([FromBody] ChangeNameRequest changeNameRequest)
		{
			_authorInfoEditor.Name = changeNameRequest.NewName;
			return new Response<EmptyResponseBody>();
		}

		[Route("connect")]
		[HttpPost]
		public async Task<Response<EmptyResponseBody>> ConnectToDocument([FromBody] DocumentConnectRequest connectRequest)
		{
			if (_documentContext.DocumentExists(connectRequest.DocumentId))
			{
				return new Response<EmptyResponseBody>();
			}

			// TODO: check errors
			await _serverSideCommunicator
				.ConnectToDocument(connectRequest.DocumentId, _authorInfoEditor.Guid);


			_documentContext.CreateNew(_documentContext.GetDocumentName(connectRequest.DocumentId), null);

			return new Response<EmptyResponseBody>();
		}

		[Route("bind/commits")]
		[HttpGet]
		public async Task<Response<IReadOnlyCollection<ClientCommit>>> BindCommits()
		{
			var commits = await GetNewCommits();
			return new Response<IReadOnlyCollection<ClientCommit>>
			{
				ResponseBody = commits,
			};
		}

		[Route("documents")]
		[HttpGet]
		public Response<IReadOnlyCollection<DocumentInfo>> GetDocuments()
		{
			// TODO: get data from another source
			var documentInfos = _documentContext.GetAllDocuments();
			return new Response<IReadOnlyCollection<DocumentInfo>>
			{
				ErrorCode = 0,
				ErrorString = string.Empty,
				ResponseBody = documentInfos,
			};
		}

		[Route("document")]
		[HttpPost]
		public Response<DocumentCreateResponse> CreateDocument(
			[FromBody] DocumentCreateRequest documentCreateRequest)
		{
			var history = _documentContext.CreateNew(documentCreateRequest.DocumentName, documentCreateRequest.Path);

			return new Response<DocumentCreateResponse>
			{
				ErrorCode = 0,
				ErrorString = string.Empty,
				ResponseBody = new DocumentCreateResponse
				{
					DocumentGuid = history.Guid,
				},
			};
		}

	}
}