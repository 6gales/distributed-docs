using System;
using DistributedDocs.Server.ClientModels;

namespace DistributedDocs.Server.Models.ServerModels
{
	internal sealed class ServerCommit
	{
		public Guid UserGuid { get; set; }
		public int CommitId { get; set; }
		public ClientCommit? Commit { get; set; }
	}
}