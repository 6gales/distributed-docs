namespace DistributedDocs.Server.Models
{
	internal sealed class Response<T>
		where T : class
	{
		public int ErrorCode { get; set; }
		public string ErrorString { get; set; } = string.Empty;
		public T? ResponseBody { get; set; }
	}
}
