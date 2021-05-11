using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DistributedDocs.Server.ClientModels;
using DistributedDocs.Server.Models.ServerModels;
using DistributedDocs.Server.Users;
using DistributedDocs.VersionHistory;
using Newtonsoft.Json;

namespace DistributedDocs.Server.Services
{
	public sealed class ServerSideCommunicator
	{
		private readonly HttpClient _httpClient = new HttpClient();
		private readonly IUserStorage _userStorage;
		private readonly Encoding _httpEncoding = Encoding.GetEncoding("ISO-8859-1");

		private readonly IAuthorInfoEditor _authorInfoEditor;

		public ServerSideCommunicator(IUserStorage userStorage, IAuthorInfoEditor authorInfoEditor)
		{
			_userStorage = userStorage;
			_authorInfoEditor = authorInfoEditor;
		}

		private HttpRequestMessage BuildMessage(HttpMethod httpMethod, IUser user, string endpoint, object? content)
		{
			var request = new HttpRequestMessage(httpMethod, $"http://{user.Host}:{user.Port}/server/{endpoint}");

			if (content != null)
			{
				request.Content = new ByteArrayContent(
					_httpEncoding.GetBytes(JsonConvert.SerializeObject(content)));
			}

			return request;
		}


		//[Route("commit")]
		//[HttpPost]
		public async Task SendCommitToGroup(ServerCommit serverCommit)
		{
			if (serverCommit.Commit != null)
			{
				foreach (var user in _userStorage.GetUserList(serverCommit.Commit.DocumentId))
				{
					await SendCommitToUser(user, serverCommit);
				}
			}
		}

		private async Task SendCommitToUser(IUser user, ServerCommit serverCommit)
		{
			var request = BuildMessage(HttpMethod.Post, user, "commit", serverCommit);
			var response = await _httpClient.SendAsync(request);

			var responseStr = await response.Content.ReadAsStringAsync();

			var data = JsonConvert.DeserializeObject<Response<EmptyResponseBody>>(responseStr);
			if (data is null || data.ErrorCode != 0)
			{
				// TODO: log somehow
			}
		}

		public async Task<List<ServerCommit>> GetHistory(Guid documentGuid)
		{
			var user = _userStorage.GetUserList(documentGuid).FirstOrDefault();
			if (user is null)
			{
				return new List<ServerCommit>();
			}

			var request = BuildMessage(HttpMethod.Get, user, $"history?documentId={documentGuid}", null);
			var response = await _httpClient.SendAsync(request);

			var responseStr = await response.Content.ReadAsStringAsync();

			var data = JsonConvert.DeserializeObject<Response<List<ServerCommit>>>(responseStr);
			if (data is null || data.ErrorCode != 0 || data.ResponseBody is null)
			{
				return new List<ServerCommit>();
			}

			return data.ResponseBody;
		}

		//[Route("connect")]
		//[HttpPost]
		public async Task<Response<EmptyResponseBody>> ConnectToDocument(Guid documentId, Guid userId)
		{
			// TODO: handle throw exception if not found
			var user = _userStorage.GetUserByGuid(documentId, userId);
			

			var requestBody = new UserConnectRequest
			{
				DocumentId = documentId,
				User = new User
				{
					UserGuid = _authorInfoEditor.Guid,
					UserName = _authorInfoEditor.Name ?? string.Empty,
				},
			};

			var request = BuildMessage(HttpMethod.Post, user, "connect", requestBody);
			var response = await _httpClient.SendAsync(request);

			var responseStr = await response.Content.ReadAsStringAsync();

			var data = JsonConvert.DeserializeObject<Response<IReadOnlyCollection<IUser>>>(responseStr);
			if (data is null)
			{
				return new Response<EmptyResponseBody>
				{
					// TODO: error codes
					ErrorCode = 45,
					ErrorString = ""
				};
			}
			if (data.ErrorCode != 0 || data.ResponseBody is null)
			{
				return new Response<EmptyResponseBody>
				{
					ErrorCode = data.ErrorCode,
					ErrorString = data.ErrorString,
				};
			}

			// Add users to document users in user storage
			foreach (var documentUser in data.ResponseBody)
			{
				_userStorage.AddUser(documentId, documentUser);
			}

			return new Response<EmptyResponseBody>();
		}

		//[Route("users")]
		//[HttpGet]
		public async Task<IReadOnlyCollection<IUser>> GetUsers(Guid documentId)
		{
			var user = _userStorage.GetUserList(documentId).FirstOrDefault();
			if (user is null)
			{
				return new List<IUser>();
			}

			var request = BuildMessage(HttpMethod.Get, user, "users", null);
			var response = await _httpClient.SendAsync(request);

			var responseStr = await response.Content.ReadAsStringAsync();

			var data = JsonConvert.DeserializeObject<Response<IReadOnlyCollection<IUser>>>(responseStr);
			if (data is null || data.ErrorCode != 0 || data.ResponseBody is null)
			{
				return new List<IUser>();
			}

			return data.ResponseBody;
		}
	}
}