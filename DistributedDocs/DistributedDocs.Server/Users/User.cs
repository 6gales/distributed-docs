using System;

namespace DistributedDocs.Server.Users
{
	internal sealed class User : IUser
	{
		public Guid UserGuid { get; set; }
		public string UserName { get; set; } = string.Empty;
		public string Host { get; set; } = string.Empty;
		public int Port { get; set; }

	}
}