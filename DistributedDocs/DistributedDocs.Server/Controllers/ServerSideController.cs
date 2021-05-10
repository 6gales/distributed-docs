using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DistributedDocs.DocumentChanges;
using DistributedDocs.Server.ClientModels;
using DistributedDocs.Server.Models.ServerModels;
using DistributedDocs.Server.Services;
using DistributedDocs.Server.Users;
using DistributedDocs.VersionHistory;
using Microsoft.AspNetCore.Mvc;

namespace DistributedDocs.Server.Controllers
{
	[Route("/server")]
	internal sealed class ServerSideController : ControllerBase
	{
		private readonly IVersionHistoryProvider<ITextDiff>_versionHistoryProvider;
		private readonly ServerSideCommunicator _serverSideCommunicator;
		private readonly DocumentContext _documentContext;
		private readonly IUserStorage _userStorage;

		public ServerSideController(IVersionHistoryProvider<ITextDiff> versionHistoryProvider,
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
		public Response<EmptyResponseBody> AddCommit([FromBody] ServerCommit serverCommit)
		{
			if (serverCommit.Commit == null)
			{
				return new Response<EmptyResponseBody>
				{
					// TODO: create enum for ErrorCodes with string values for ErrorString
					ErrorCode = 1,
					ErrorString = "No such file",
					ResponseBody = null,
				};
			}

			_documentContext.EditDocument(serverCommit.Commit.DocumentId, serverCommit);

			return new Response<EmptyResponseBody>();
		}

		[Route("history")]
		[HttpGet]
		public Response<IReadOnlyCollection<ServerCommit>> GetHistory([FromQuery] Guid documentGuid)
		{
			var history = _documentContext.GetHistory(documentGuid);
			return new Response<IReadOnlyCollection<ServerCommit>>
			{
				ResponseBody = history,
			};
		}

		[Route("connect")]
		[HttpPost]
		public Response<IReadOnlyCollection<IUser>> ConnectUser([FromBody] UserConnectRequest userConnectRequest)
		{
			// TODO: handle user connection
			if (userConnectRequest.User is null)
			{
				return new Response<IReadOnlyCollection<IUser>>
				{
					ErrorCode = 2,
					ErrorString = "Has no user",
					ResponseBody = null,
				};
			}

			_userStorage.AddUser(userConnectRequest.DocumentId, userConnectRequest.User);

			return GetUsers(userConnectRequest.DocumentId);
		}

		[Route("users")]
		[HttpGet]
		public Response<IReadOnlyCollection<IUser>> GetUsers([FromQuery]Guid documentId)
		{
			return new Response<IReadOnlyCollection<IUser>>
			{
				ErrorCode = 0,
				ErrorString = string.Empty,
				ResponseBody = _userStorage.GetUserList(documentId),

			};
		}

	}
}