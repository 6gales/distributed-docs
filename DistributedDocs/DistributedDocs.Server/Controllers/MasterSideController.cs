using System.Collections.Generic;
using DistributedDocs.Server.Models;
using DistributedDocs.Server.Models.ServerModels;
using DistributedDocs.Server.Models.ServerModels.MasterModels;
using DistributedDocs.VersionHistory;
using Microsoft.AspNetCore.Mvc;

namespace DistributedDocs.Server.Controllers
{
	[Route("/master")]
	internal sealed class MasterSideController : ControllerBase
	{
		private readonly IConcurrentVersionHistory _concurrentVersionHistory;

		public MasterSideController(IConcurrentVersionHistory concurrentVersionHistory)
		{
			_concurrentVersionHistory = concurrentVersionHistory;
		}

		[Route("history")]
		[HttpGet]
		public Response<List<ServerCommit>> GetHistory()
		{
			return new Response<List<ServerCommit>>();
		}

		[Route("users")]
		[HttpGet]
		public Response<List<UserGetResponse>> GetUsers()
		{
			return new Response<List<UserGetResponse>>();
		}

		[Route("connect")]
		[HttpGet]
		public Response<UserCreateResponse> GetUsers([FromQuery] string docId)
		{
			return new Response<UserCreateResponse>();
		}

	}
}