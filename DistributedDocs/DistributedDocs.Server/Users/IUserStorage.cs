using System;
using System.Collections.Generic;
using DistributedDocs.Server.Models.ServerModels;

namespace DistributedDocs.Server.Users
{
	internal interface IUserStorage
	{
		public User AddUser(string userName, UserStatus userStatus);
		public User GetUserByGuid(Guid userGuid);
		public List<User> GetUserList();
		public void DeleteUser(Guid userGuid);
	}
}