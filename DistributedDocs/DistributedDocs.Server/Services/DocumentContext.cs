using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DistributedDocs.DocumentChanges;
using DistributedDocs.Server.ClientModels;
using DistributedDocs.Server.Models.ServerModels;
using DistributedDocs.Server.Users;
using DistributedDocs.VersionHistory;

namespace DistributedDocs.Server.Services
{
	internal sealed class DocumentContext
	{
		private readonly Dictionary<Guid, IConcurrentVersionHistory<ITextDiff>> _documents =
			new Dictionary<Guid, IConcurrentVersionHistory<ITextDiff>>();

		private readonly IUserStorage _userStorage;
		private readonly ServerSideCommunicator _serverSideCommunicator;
		private readonly IVersionHistoryProvider<ITextDiff> _versionHistoryProvider;

		public DocumentContext(IVersionHistoryProvider<ITextDiff> versionHistoryProvider,
			IUserStorage userStorage,
			ServerSideCommunicator serverSideCommunicator)
		{
			_versionHistoryProvider = versionHistoryProvider;
			_userStorage = userStorage;
			_serverSideCommunicator = serverSideCommunicator;
		}

		public IConcurrentVersionHistory<ITextDiff> CreateNew(string name, string? path = null)
		{
			var versionHistory = _versionHistoryProvider.ProvideHistory(name, path);

			_documents.Add(versionHistory.Guid, versionHistory);

			return versionHistory;
		}

		public async Task EditDocument(Guid documentId, ClientCommit commit)
		{
			if (!_documents.TryGetValue(documentId, out var versionHistory))
			{
				return;
			}

			var diff = new TextDiff(commit.BeginIndex, commit.EndIndex, commit.String);
			var localCommit = versionHistory.CommitChange(diff);

			// send to another server sides local commit
			var serverCommit = new ServerCommit
			{
				Commit = commit,
				CommitId = localCommit.Id,
				UserGuid = _userStorage.Self.UserGuid,
			};

			await _serverSideCommunicator.SendCommitToGroup(serverCommit);
		}

		public async Task EditDocument(Guid documentId, ServerCommit commit)
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
				await _serverSideCommunicator.SendCommitToGroup(commit);
			}
		}

		public IReadOnlyCollection<ServerCommit> GetHistory(Guid documentId)
		{
			if (!_documents.TryGetValue(documentId, out var version))
			{
				return new List<ServerCommit>();
			}

			// TODO: get history from version
			// version.

			return new List<ServerCommit>();
		}

		public IReadOnlyCollection<DocumentInfo> GetDocuments()
		{
			var docs = new List<DocumentInfo>();
			foreach (var concurrentVersionHistory in _documents)
			{
				docs.Add(new DocumentInfo
				{
					DocumentId = concurrentVersionHistory.Key,
					// TODO: concurrent version should provide filename
					DocumentName = "bl",
				});
			}

			return docs;
		}
	}
}