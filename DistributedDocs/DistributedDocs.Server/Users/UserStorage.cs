using System;
using System.Collections.Generic;
using System.Linq;

namespace DistributedDocs.Server.Users
{
	internal sealed class UserStorage : IUserStorage
	{
		private readonly Dictionary<Guid, IUser> _users = new Dictionary<Guid, IUser>();

		public void AddUser(IUser user)
		{
			_users.Add(user.UserGuid, user);
		}

		public IUser GetUserByGuid(Guid userGuid)
		{
			if (_users.TryGetValue(userGuid, out var user))
			{
				return user;
			}

			throw new ArgumentException($"User with specified id not found: {userGuid}");
		}

		public IReadOnlyCollection<IUser> GetUserList()
		{
			return _users.Values.ToList();
		}

		public void DeleteUser(Guid userGuid)
		{
			if (_users.ContainsKey(userGuid))
			{
				_users.Remove(userGuid);
			}
		}
	}
}