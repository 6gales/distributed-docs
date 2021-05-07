using System;

namespace DistributedDocs.Server.ClientModels
{
	public sealed class ClientCommit
	{
		public Guid DocumentId { get; set; }
		public int BeginIndex { get; set; }
		public int EndIndex { get; set; }
		public string String { get; set; } = string.Empty;
	}
}