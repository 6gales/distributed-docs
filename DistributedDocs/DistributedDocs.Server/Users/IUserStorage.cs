using System;
using System.Collections.Generic;

namespace DistributedDocs.Server.Users
{
	public interface IUserStorage
	{
		public void AddUser(Guid documentId, IUser user);
		public IUser GetUserByGuid(Guid documentId, Guid userGuid);
		public IReadOnlyCollection<IUser> GetUserList(Guid documentId);
		public void DeleteUser(Guid documentId, Guid userGuid);
	}
}