using System;
using DistributedDocs.Server.Models.ServerModels;

namespace DistributedDocs.Server.Users
{
	internal sealed class User
	{
		public Guid UserGuid { get; set; }
		public string UserName { get; set; }
		public UserStatus UserStatus { get; set; }

	}
}