using System;
using System.Collections.Generic;
using System.Linq;

namespace DistributedDocs.Server.Users
{
	public sealed class UserStorage : IUserStorage
	{
		private readonly Dictionary<Guid, Dictionary<Guid, IUser>> _documentUsers =
			new Dictionary<Guid, Dictionary<Guid, IUser>>();

		private Dictionary<Guid, IUser> GetOrCreate(Guid documentId)
		{
			if (!_documentUsers.TryGetValue(documentId, out var users))
			{
				users = new Dictionary<Guid, IUser>();
				_documentUsers[documentId] = users;
			}

			return users;
		}

		public void AddUser(Guid documentId, IUser user)
		{
			GetOrCreate(documentId).Add(user.UserGuid, user);
		}

		public IUser GetUserByGuid(Guid documentId, Guid userGuid)
		{
			if (GetOrCreate(documentId).TryGetValue(userGuid, out var user))
			{
				return user;
			}

			throw new ArgumentException($"User with specified id not found: {userGuid}");
		}

		public IReadOnlyCollection<IUser> GetUserList(Guid documentId)
		{
			return GetOrCreate(documentId).Values.ToList();
		}

		public void DeleteUser(Guid documentId, Guid userGuid)
		{
			GetOrCreate(documentId).Remove(userGuid);
		}
	}
}