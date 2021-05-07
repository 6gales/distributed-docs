using System;
using System.Net;

namespace DistributedDocs.Server.Users
{
	public interface IUser
	{
		public Guid UserGuid { get; set; }
		public string UserName { get; set; }
		public SocketAddress? SocketAddress { get; set; }
	}
}