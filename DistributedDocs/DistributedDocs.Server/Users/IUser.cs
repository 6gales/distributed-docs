using System;

namespace DistributedDocs.Server.Users
{
	public interface IUser
	{
		public Guid UserGuid { get; set; }
		public string UserName { get; set; }
		public string Host { get; set; }
		public int Port { get; set; }
	}
}