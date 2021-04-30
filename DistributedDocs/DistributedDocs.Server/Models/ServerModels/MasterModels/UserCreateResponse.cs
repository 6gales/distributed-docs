using System;

namespace DistributedDocs.Server.Models.ServerModels.MasterModels
{
	internal sealed class UserCreateResponse
	{
		public Guid UserGuid{ get; set; }
		public UserStatus UserStatus { get; set; }
	}
}