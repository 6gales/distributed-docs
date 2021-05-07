using System.Collections.Generic;
using System.Threading.Tasks;
using DistributedDocs.Server.ClientModels;
using DistributedDocs.Server.Models.ServerModels;
using DistributedDocs.Server.Users;
using DistributedDocs.VersionHistory;
using Microsoft.AspNetCore.Mvc;

namespace DistributedDocs.Server.Controllers
{
	[Route("/server")]
	internal sealed class ServerSideController : ControllerBase
	{
		private readonly IConcurrentVersionHistory _concurrentVersionHistory;

		public ServerSideController(IConcurrentVersionHistory concurrentVersionHistory)
		{
			_concurrentVersionHistory = concurrentVersionHistory;
		}

		[Route("commit")]
		[HttpPost]
		public async Task<Response<EmptyResponseBody>> AddCommit([FromBody] ServerCommit serverCommit)
		{
			return new Response<EmptyResponseBody>();
			//_concurrentVersionHistory.AddCommit();
		}

		[Route("commit")]
		[HttpPut]
		public async Task<Response<EmptyResponseBody>> ChangeCommit([FromBody] ServerCommit serverCommit)
		{
			return new Response<EmptyResponseBody>();
			//_concurrentVersionHistory.AddCommit();
		}

		[Route("history")]
		[HttpGet]
		public async Task<Response<List<ServerCommit>>> GetHistory()
		{
			return new Response<List<ServerCommit>>();
		}

		[Route("connect")]
		[HttpPost]
		public async Task<Response<IReadOnlyCollection<IUser>>> ConnectUser([FromBody] UserConnectRequest userConnectRequest)
		{
			// TODO: handle user connection

			return await GetUsers();
		}

		[Route("users")]
		[HttpGet]
		public async Task<Response<IReadOnlyCollection<IUser>>> GetUsers()
		{
			return new Response<IReadOnlyCollection<IUser>>();
		}

	}
}