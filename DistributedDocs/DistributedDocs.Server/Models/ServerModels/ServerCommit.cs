using System;
using DistributedDocs.Server.ClientModels;

namespace DistributedDocs.Server.Models.ServerModels
{
	public sealed class ServerCommit
	{
		public Guid UserGuid { get; set; }
		public string? UserName { get; set; }
		public int CommitId { get; set; }
		public ClientCommit? Commit { get; set; }
	}
}