using System;

namespace DistributedDocs.Server.Models.ServerModels
{
	internal sealed class ServerCommit
	{
		public Guid UserGuid { get; set; }
		public int CommitId { get; set; }
		public int BeginIndex { get; set; }
		public int EndIndex { get; set; }
		public string String { get; set; } = string.Empty;
	}
}