using System;
using DistributedDocs.Server.Users;

namespace DistributedDocs.Server.Models.ServerModels
{
	internal sealed class UserConnectRequest
	{
		public IUser? User { get; set; }
		public Guid DocumentId { get; set; }
	}
}