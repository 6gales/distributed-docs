using System;
using System.Collections.Generic;
using DistributedDocs.Server.ClientModels;
using DistributedDocs.Server.Models.ServerModels;
using DistributedDocs.Server.Services;
using DistributedDocs.Server.Users;
using Microsoft.AspNetCore.Mvc;

namespace DistributedDocs.Server.Controllers
{
	[ApiController]
    [Route("server")]
    public sealed class ServerSideController : ControllerBase
	{
		private readonly DocumentContext _documentContext;
		private readonly IUserStorage _userStorage;

		public ServerSideController(DocumentContext documentContext,
			IUserStorage userStorage)
		{
			_documentContext = documentContext;
			_userStorage = userStorage;
		}

		/// <summary>
		/// Add commit to server side history
		/// </summary>
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

		/// <summary>
		/// Get history
		/// </summary>
		[Route("history")]
		[HttpGet]
		public Response<IReadOnlyCollection<ServerCommit>> GetHistory([FromQuery] Guid documentId)
		{
			try
			{
				var history = _documentContext.GetHistory(documentId);
				return new Response<IReadOnlyCollection<ServerCommit>>
				{
					ResponseBody = history,
				};
			}
			catch (ArgumentException e)
			{
				return new Response<IReadOnlyCollection<ServerCommit>>
				{
					ErrorCode = 155,
					ErrorString = e.Message,
					ResponseBody = null
				};
			}


		}

		/// <summary>
		/// User connect request register self in list of users and gets list of document's users
		/// </summary>
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

			var user = userConnectRequest.User;
			user.Host = Request.HttpContext.Connection.RemoteIpAddress.ToString();
			// TODO: use port form request
			//user.Port = Request.HttpContext.Connection.RemotePort;

			_userStorage.AddUser(userConnectRequest.DocumentId, user);

			return GetUsers(userConnectRequest.DocumentId);
		}

		/// <summary>
		/// User connect request gets list of document's users
		/// </summary>
		[Route("users")]
		[HttpGet]
		public Response<IReadOnlyCollection<IUser>> GetUsers([FromQuery]Guid documentId)
		{
			var users = _userStorage.GetUserList(documentId);
			return new Response<IReadOnlyCollection<IUser>>
			{
				ErrorCode = 0,
				ErrorString = string.Empty,
				ResponseBody = users,

			};
		}

	}
}