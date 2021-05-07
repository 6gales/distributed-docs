using System;
using System.Collections.Generic;
using DistributedDocs.DocumentChanges;
using DistributedDocs.Server.ClientModels;
using DistributedDocs.Server.Models.ServerModels;
using DistributedDocs.Server.Users;
using DistributedDocs.VersionHistory;

namespace DistributedDocs.Server.Services
{
	internal sealed class DocumentContext
	{
		private Dictionary<Guid, IConcurrentVersionHistory<ITextDiff>> _documents =
			new Dictionary<Guid, IConcurrentVersionHistory<ITextDiff>>();

		private readonly IUserStorage _userStorage;


		private readonly IVersionHistoryProvider<ITextDiff> _versionHistoryProvider;

		public DocumentContext(IVersionHistoryProvider<ITextDiff> versionHistoryProvider, IUserStorage userStorage)
		{
			_versionHistoryProvider = versionHistoryProvider;
			_userStorage = userStorage;
		}

		public IConcurrentVersionHistory<ITextDiff> CreateNew(string name, string? path = null)
		{
			var versionHistory = _versionHistoryProvider.ProvideHistory(name, path);

			_documents.Add(versionHistory.Guid, versionHistory);

			return versionHistory;
		}

		public void EditDocument(Guid documentId, ClientCommit commit)
		{
			if (!_documents.TryGetValue(documentId, out var versionHistory))
			{
				return;
			}

			var diff = new TextDiff(commit.BeginIndex, commit.EndIndex, commit.String);
			var localCommit = versionHistory.CommitChange(diff);

			// send to another server sides local commit
		}

		public void EditDocument(Guid documentId, ServerCommit commit)
		{
			if (!_documents.TryGetValue(documentId, out var versionHistory))
			{
				return;
			}

			if (commit.Commit != null)
			{
				var diff = new TextDiff(commit.Commit.BeginIndex, 
					commit.Commit.EndIndex, commit.Commit.String);

				var newCommit = new Commit<ITextDiff>(commit.CommitId, 
					new AuthorInfo(commit.UserGuid, string.Empty), diff);

				versionHistory.AddCommit(newCommit);

				// send to client somehow
			}
		}
	}
}