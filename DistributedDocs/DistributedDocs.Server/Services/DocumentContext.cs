using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DistributedDocs.DocumentChanges;
using DistributedDocs.Server.ClientModels;
using DistributedDocs.Server.Models.ServerModels;
using DistributedDocs.VersionHistory;

namespace DistributedDocs.Server.Services
{
	internal sealed class DocumentContext
	{
		private readonly Dictionary<Guid, IConcurrentVersionHistory<ITextDiff>> _documents =
			new Dictionary<Guid, IConcurrentVersionHistory<ITextDiff>>();

		private readonly Dictionary<Guid, DocumentInfo> _remoteDocuments = 
			new Dictionary<Guid, DocumentInfo>();



		//private readonly IUserStorage _userStorage;
		private readonly ServerSideCommunicator _serverSideCommunicator;
		private readonly IVersionHistoryProvider<ITextDiff> _versionHistoryProvider;
		private readonly IAuthorInfoEditor _authorInfoEditor;


		public event Action<ServerCommit>? OnCommit;

		public DocumentContext(IVersionHistoryProvider<ITextDiff> versionHistoryProvider,
			ServerSideCommunicator serverSideCommunicator,
			IAuthorInfoEditor authorInfoEditor)
		{
			_versionHistoryProvider = versionHistoryProvider;
			_serverSideCommunicator = serverSideCommunicator;
			_authorInfoEditor = authorInfoEditor;
		}

		public IConcurrentVersionHistory<ITextDiff> CreateNew(string name, string? path = null)
		{
			var versionHistory = _versionHistoryProvider.ProvideHistory(name, path);

			_documents.Add(versionHistory.Guid, versionHistory);

			return versionHistory;
		}

		public IConcurrentVersionHistory<ITextDiff> CreateNew(Guid documentGuid, 
			string documentName, 
			IReadOnlyCollection<ServerCommit> history)
		{
			// TODO: use new functional

			//var versionHistory = _versionHistoryProvider.ProvideHistory(name, path);

			//_documents.Add(versionHistory.Guid, versionHistory);

			return null;
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
				UserGuid = _authorInfoEditor.Guid,
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

			OnCommit?.Invoke(commit);
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
			var docs = _documents
				.Select(
					d => new DocumentInfo
					{
						DocumentId = d.Key,
						// TODO: concurrent version should provide filename
						DocumentName = "bl",
					})
				.ToList();

			return docs;
		}

		public IReadOnlyCollection<DocumentInfo> GetAllDocuments()
		{
			var docs = _documents
				.Select(
					d => new DocumentInfo
					{
						DocumentId = d.Key,
						// TODO: concurrent version should provide filename
						DocumentName = "bl",
					})
				.Concat(_remoteDocuments
					.Select(d => d.Value)
					.Where(d => !DocumentExists(d.DocumentId)))
				.ToList();

			return docs;
		}

		public bool DocumentExists(Guid documentId)
		{
			return _documents.ContainsKey(documentId);
		}

		public string GetDocumentName(Guid documentGuid)
		{
			if (_documents.TryGetValue(documentGuid, out var history))
			{
				// TODO: add get name
				return history.Guid.ToString();
			}

			if (_remoteDocuments.TryGetValue(documentGuid, out var docInfo))
			{
				return docInfo.DocumentName;
			}

			return string.Empty;
		}

		public void AddRemoteDocument(Guid documentId, DocumentInfo documentInfo)
		{
			_remoteDocuments.Add(documentId, documentInfo);
		}
	}
}