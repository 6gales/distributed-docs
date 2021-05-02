using System;
using System.Collections.Generic;
using System.Linq;
using DistributedDocs.Server.Models.ServerModels;

namespace DistributedDocs.Server.Users
{
	internal sealed class UserStorage : IUserStorage
	{
		private readonly Dictionary<Guid, User> _users = new Dictionary<Guid, User>();

		public User AddUser(string userName, UserStatus userStatus)
		{
			var userGuid = new Guid();
			while (_users.ContainsKey(userGuid))
			{
				userGuid = Guid.NewGuid();
			}

			User newUser = new User
			{
				UserGuid = userGuid,
				UserName = userName,
				UserStatus = userStatus
			};

			_users.Add(userGuid, newUser);

			return newUser;
		}

		public User GetUserByGuid(Guid userGuid)
		{
			return _users.FirstOrDefault(pair => pair.Key == userGuid).Value;
		}

		public List<User> GetUserList()
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