using System;

namespace DistributedDocs.Server.ClientModels
{
	public sealed class DocumentInfo
	{
		public Guid DocumentId { get; set; }
		public string DocumentName { get; set; } = string.Empty;
	}
}