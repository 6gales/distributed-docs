using System;
using System.Collections.Generic;

namespace DistributedDocs.Server.Users
{
	internal interface IUserStorage
	{
		public void AddUser(IUser user);
		public IUser GetUserByGuid(Guid userGuid);
		public IReadOnlyCollection<IUser> GetUserList();
		public void DeleteUser(Guid userGuid);
	}
}