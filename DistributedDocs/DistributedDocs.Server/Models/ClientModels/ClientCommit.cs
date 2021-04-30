namespace DistributedDocs.Server.Models.ClientModels
{
	internal sealed class ClientCommit
	{
		public int BeginIndex { get; set; }
		public int EndIndex { get; set; }
		public string String { get; set; } = string.Empty;
	}
}