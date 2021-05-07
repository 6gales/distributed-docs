using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DistributedDocs.Server.ClientModels;
using DistributedDocs.Server.Models.ServerModels;
using DistributedDocs.Server.Users;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DistributedDocs.Server.Services
{
	internal sealed class ServerSideCommunicator
	{
		private readonly HttpClient _httpClient = new HttpClient();
		private readonly IUserStorage _userStorage;
//		private readonly JsonConverter _jsonConverter = new Json
		private readonly Encoding _httpEncoding = Encoding.GetEncoding("ISO-8859-1");

		public ServerSideCommunicator(IUserStorage userStorage)
		{
			_userStorage = userStorage;
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
			foreach (var user in _userStorage.GetUserList())
			{
				await SendCommitToUser(user, serverCommit);
			}
		}

		private async Task SendCommitToUser(IUser user, ServerCommit serverCommit)
		{
			var request = BuildMessage(HttpMethod.Post, user, "commit", serverCommit);
			await _httpClient.SendAsync(request);
		}

		//[Route("commit")]
		//[HttpPut]
		//public async Task<Response<EmptyResponseBody>> ChangeCommit(ServerCommit serverCommit)
		//{
		//	return new Response<EmptyResponseBody>();
		//	//_concurrentVersionHistory.AddCommit();
		//}

		//[Route("history")]
		//[HttpGet]
		public async Task<List<ServerCommit>> GetHistory()
		{
			var user = _userStorage.GetUserList().FirstOrDefault();
			if (user is null)
			{
				return new List<ServerCommit>();
			}

			var request = BuildMessage(HttpMethod.Get, user, "history", null);
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
		public async Task<Response<IReadOnlyCollection<IUser>>> ConnectUser([FromBody] UserConnectRequest userConnectRequest)
		{
			return await GetUsers();
		}

		//[Route("users")]
		//[HttpGet]
		public async Task<Response<IReadOnlyCollection<IUser>>> GetUsers()
		{
			return new Response<IReadOnlyCollection<IUser>>();
		}
	}
}