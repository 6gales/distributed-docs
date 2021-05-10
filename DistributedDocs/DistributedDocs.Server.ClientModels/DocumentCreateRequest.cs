namespace DistributedDocs.Server.ClientModels
{
	public sealed class DocumentCreateRequest
	{
		public string DocumentName { get; set; } = string.Empty;
		public string? Path { get; set; }
	}
}