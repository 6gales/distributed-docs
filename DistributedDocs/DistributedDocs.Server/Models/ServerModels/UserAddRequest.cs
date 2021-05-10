using DistributedDocs.Server.Users;

namespace DistributedDocs.Server.Models.ServerModels
{
	internal sealed class UserAddRequest
	{
		public IUser? User { get; set; }
	}
}