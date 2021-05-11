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
    [ApiController]
	[Route("client")]
    public sealed class ClientSideController : ControllerBase
	{
		private readonly ServerSideCommunicator _serverSideCommunicator;
		private readonly DocumentContext _documentContext;
		private readonly IAuthorInfoEditor _authorInfoEditor;

		private readonly List<ClientCommit> _commitsToSend = new List<ClientCommit>();
		private readonly object _commitsSyncRoot = new object();
		private readonly ManualResetEvent _event = new ManualResetEvent(false);

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

		/// <summary>
		/// Push new ClientCommit server history
		/// </summary>
		[HttpPost("commit")]
		public async Task<Response<EmptyResponseBody>> AddCommit([FromBody] ClientCommit clientCommit)
		{
			await _documentContext.EditDocument(clientCommit.DocumentId, clientCommit);
			return new Response<EmptyResponseBody>();
		}

		/// <summary>
		/// Change User name that will be used in history commit like author 
		/// </summary>
		[HttpPost("user")]
		public Response<EmptyResponseBody> ChangeName([FromBody] ChangeNameRequest changeNameRequest)
		{
			_authorInfoEditor.Name = changeNameRequest.NewName;
			return new Response<EmptyResponseBody>();
		}

		/// <summary>
		/// Uses for connect to document with specified id
		/// </summary>
		[HttpPost("connect")]
		public async Task<Response<EmptyResponseBody>> ConnectToDocument([FromBody] DocumentConnectRequest connectRequest)
		{
			if (_documentContext.DocumentExists(connectRequest.DocumentId))
			{
				_documentContext.LoadHistory(connectRequest.DocumentId);
				return new Response<EmptyResponseBody>();

			}

			// TODO: check errors
			await _serverSideCommunicator
				.ConnectToDocument(connectRequest.DocumentId, _authorInfoEditor.Guid);

			var history = await _serverSideCommunicator.GetHistory(connectRequest.DocumentId);
			_documentContext.CreateNew(connectRequest.DocumentId, 
				_documentContext.GetDocumentName(connectRequest.DocumentId), 
				history);

			_documentContext.LoadHistory(connectRequest.DocumentId);

			return new Response<EmptyResponseBody>();
		}

		/// <summary>
		/// Long-polling request for taking newest commits from server
		/// </summary>
		[HttpGet("bind/commits")]
		public async Task<Response<IReadOnlyCollection<ClientCommit>>> BindCommits()
		{
			var commits = await GetNewCommits();
			return new Response<IReadOnlyCollection<ClientCommit>>
			{
				ResponseBody = commits,
			};
		}

		/// <summary>
		/// Get list of documents with their names and ids
		/// </summary>
		[HttpGet("documents")]
		public Response<IReadOnlyCollection<DocumentInfo>> GetDocuments()
        {
            var documentInfos = _documentContext.GetAllDocuments();
            return new Response<IReadOnlyCollection<DocumentInfo>>
            {
                ErrorCode = 0,
                ErrorString = string.Empty,
                ResponseBody = documentInfos,
		    };
        }

		/// <summary>
		/// Create document with name and path return document id
		/// </summary>
		[HttpPost("document")]
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
					DocumentId = history.Guid,
				},
			};
		}

	}
}