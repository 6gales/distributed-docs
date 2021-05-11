using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DistributedDocs.DocumentChanges;
using DistributedDocs.Server.ClientModels;
using DistributedDocs.Server.Models.ServerModels;
using DistributedDocs.VersionHistory;
using DistributedDocs.Server.Extensions;

namespace DistributedDocs.Server.Services
{
	public sealed class DocumentContext
	{
		private readonly Dictionary<Guid, IConcurrentVersionHistory<ITextDiff>> _documents =
			new Dictionary<Guid, IConcurrentVersionHistory<ITextDiff>>();

		private readonly Dictionary<Guid, DocumentInfo> _remoteDocuments = 
			new Dictionary<Guid, DocumentInfo>();

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

		public IConcurrentVersionHistory<ITextDiff> CreateNew(Guid documentId, 
			string documentName, 
			IReadOnlyCollection<ServerCommit> history)
		{
			var versionHistory = _versionHistoryProvider.FromExisting(documentId, documentName, 
				history.Select(c => c.ToHistoryCommit()));

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
				UserGuid = _authorInfoEditor.Guid,
				UserName = _authorInfoEditor.Name,
			};

			await _serverSideCommunicator.SendCommitToGroup(serverCommit);
		}

		public void EditDocument(Guid documentId, ServerCommit commit)
		{
			if (!_documents.TryGetValue(documentId, out var versionHistory))
			{
				return;
			}

			versionHistory.AddCommit(commit.ToHistoryCommit());

			OnCommit?.Invoke(commit);
		}

		public IReadOnlyCollection<ServerCommit> GetHistory(Guid documentId)
		{
			if (!_documents.TryGetValue(documentId, out var version))
			{
				return new List<ServerCommit>();
			}

			return version.History
				.Select(c => c.FromHistoryCommit(documentId)).ToList();
		}

		public IReadOnlyCollection<DocumentInfo> GetDocuments()
		{
			var docs = _documents
				.Select(
					d => new DocumentInfo
					{
						DocumentId = d.Key,
						DocumentName = d.Value.Name,
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
						DocumentName = d.Value.Name,
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
				return history.Name;
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