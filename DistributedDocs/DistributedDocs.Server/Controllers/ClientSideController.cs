using System.Collections.Generic;
using DistributedDocs.Server.Models;
using DistributedDocs.Server.Models.ClientModels;
using DistributedDocs.Server.Models.ServerModels;
using DistributedDocs.VersionHistory;
using Microsoft.AspNetCore.Mvc;

namespace DistributedDocs.Server.Controllers
{
	[Route("/client")]
	internal sealed class ClientSideController : ControllerBase
	{
		private readonly IConcurrentVersionHistory _concurrentVersionHistory;

		public ClientSideController(IConcurrentVersionHistory concurrentVersionHistory)
		{
			_concurrentVersionHistory = concurrentVersionHistory;
		}

		[Route("commit")]
		[HttpPost]
		public Response<EmptyResponseBody> AddCommit([FromBody] ClientCommit clientCommit)
		{
			return new Response<EmptyResponseBody>();
		}

		[Route("bind")]
		[HttpGet]
		public Response<List<ClientCommit>> Bind()
		{
			return new Response<List<ClientCommit>>();
		}

	}
}