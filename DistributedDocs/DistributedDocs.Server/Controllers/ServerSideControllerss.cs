using DistributedDocs.Server.Models;
using DistributedDocs.Server.Models.ServerModels;
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
		public Response<EmptyResponseBody> AddCommit([FromBody] ServerCommit serverCommit)
		{
			return new Response<EmptyResponseBody>();
			//_concurrentVersionHistory.AddCommit();
		}

		[Route("commit")]
		[HttpPut]
		public Response<EmptyResponseBody> ChangeCommit([FromBody] ServerCommit serverCommit)
		{
			return new Response<EmptyResponseBody>();
			//_concurrentVersionHistory.AddCommit();
		}

		[Route("deputy")]
		[HttpPut]
		public Response<EmptyResponseBody> ChangeDeputy([FromBody] ChangeDeputyRequest changeDeputyRequest)
		{
			return new Response<EmptyResponseBody>();
		}

		[Route("user")]
		[HttpPost]
		public Response<EmptyResponseBody> AddUser([FromBody] UserAddRequest userAddRequest)
		{
			return new Response<EmptyResponseBody>();
		}

	}
}