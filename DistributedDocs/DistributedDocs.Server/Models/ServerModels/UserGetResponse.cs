using DistributedDocs.Server.Users;

namespace DistributedDocs.Server.Models.ServerModels
{
	internal sealed class UserGetResponse
	{
		public IUser? User { get; set; }
	}
}