using System.Collections.Generic;
using System.Threading.Tasks;
using DistributedDocs.DocumentChanges;
using DistributedDocs.VersionHistory;
using DistributedDocs.Server.ClientModels;
using DistributedDocs.Server.Services;
using DistributedDocs.Server.Users;
using Microsoft.AspNetCore.Mvc;

namespace DistributedDocs.Server.Controllers
{
	[Route("/client")]
	internal sealed class ClientSideController : ControllerBase
	{
		private readonly IVersionHistoryProvider<ITextDiff> _versionHistoryProvider;
		private readonly ServerSideCommunicator _serverSideCommunicator;
		private readonly IUserStorage _userStorage;
		private readonly DocumentContext _documentContext;

		public ClientSideController(IVersionHistoryProvider<ITextDiff> versionHistoryProvider,
			ServerSideCommunicator serverSideCommunicator,
			DocumentContext documentContext,
			IUserStorage userStorage)
		{
			_versionHistoryProvider = versionHistoryProvider;
			_serverSideCommunicator = serverSideCommunicator;
			_documentContext = documentContext;
			_userStorage = userStorage;
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
			_userStorage.Self.UserName = changeNameRequest.NewName;
			return new Response<EmptyResponseBody>();
		}

		[Route("connect")]
		[HttpPost]
		public async Task<Response<EmptyResponseBody>> ConnectToDocument([FromBody] DocumentConnectRequest connectRequest)
		{
			// TODO: only send request without creating doc
			var users = await _serverSideCommunicator.ConnectToDocument(connectRequest.DocumentId, _userStorage.Self.UserGuid);
			foreach (var user in users)
			{
				_userStorage.AddUser(user);	
			}

			return new Response<EmptyResponseBody>();
		}

		[Route("bind/commits")]
		[HttpGet]
		public async Task<Response<IReadOnlyCollection<ClientCommit>>> BindCommits()
		{
			return new Response<IReadOnlyCollection<ClientCommit>>();
		}

		[Route("documents")]
		[HttpGet]
		public Response<IReadOnlyCollection<DocumentInfo>> GetDocuments()
		{

			return new Response<IReadOnlyCollection<DocumentInfo>>();
		}

	}
}