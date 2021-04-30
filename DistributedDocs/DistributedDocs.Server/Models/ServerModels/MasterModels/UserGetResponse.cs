using System;

namespace DistributedDocs.Server.Models.ServerModels.MasterModels
{
	internal sealed class UserGetResponse
	{
		public Guid UserGuid { get; set; }
		public UserStatus UserStatus { get; set; }
	}
}