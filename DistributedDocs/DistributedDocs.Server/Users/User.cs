using System;
using System.Net;

namespace DistributedDocs.Server.Users
{
	internal sealed class User : IUser
	{
		public Guid UserGuid { get; set; }
		public string UserName { get; set; } = string.Empty;
		public SocketAddress? SocketAddress { get; set; }

	}
}