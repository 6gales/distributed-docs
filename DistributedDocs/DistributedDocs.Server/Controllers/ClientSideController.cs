using System.Collections.Generic;
using System.Threading.Tasks;
using DistributedDocs.VersionHistory;
using DistributedDocs.Server.ClientModels;
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
		public async Task<Response<EmptyResponseBody>> AddCommit([FromBody] ClientCommit clientCommit)
		{
			return new Response<EmptyResponseBody>();
		}

		[Route("user")]
		[HttpPost]
		public async Task<Response<EmptyResponseBody>> ChangeName([FromBody] ChangeNameRequest changeNameRequest)
		{

			return new Response<EmptyResponseBody>();
		}

		[Route("connect")]
		[HttpPost]
		public async Task<Response<EmptyResponseBody>> ConnectToDocument([FromBody] DocumentConnectRequest connectRequest)
		{
			return new Response<EmptyResponseBody>();
		}

		[Route("bind/commits")]
		[HttpGet]
		public async Task<Response<IReadOnlyCollection<ClientCommit>>> BindCommits()
		{
			return new Response<IReadOnlyCollection<ClientCommit>>();
		}

		[Route("bind/documents")]
		[HttpGet]
		public async Task<Response<IReadOnlyCollection<DocumentInfo>>> BindDocuments()
		{
			return new Response<IReadOnlyCollection<DocumentInfo>>();
		}

	}
}