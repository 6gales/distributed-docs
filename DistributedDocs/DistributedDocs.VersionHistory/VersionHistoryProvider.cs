using System;
using System.Collections.Generic;
using DistributedDocs.DocumentChanges;
using DistributedDocs.FileSystem;

namespace DistributedDocs.VersionHistory
{
    public sealed class VersionHistoryProvider : IVersionHistoryProvider<ITextDiff>
    {
        private readonly IAuthorProvider _authorProvider;
        private readonly IFileSynchronizerProvider<ITextDiff> _synchronizerProvider;

        public VersionHistoryProvider(
            IAuthorProvider authorProvider,
            IFileSynchronizerProvider<ITextDiff> synchronizerProvider)
        {
            _authorProvider = authorProvider;
            _synchronizerProvider = synchronizerProvider;
        }

        public IConcurrentVersionHistory<ITextDiff> ProvideHistory(string name, string? path = null)
        {
            return new ConcurrentVersionHistory<ITextDiff>(
                _authorProvider.ProvideAuthor(),
                name,
                new CommitMerger(_synchronizerProvider.ProvideFileSynchronizer(name, path)));
        }

        public IConcurrentVersionHistory<ITextDiff> FromExisting(Guid historyId, string name, IEnumerable<ICommit<ITextDiff>> existingCommits)
        {
            return new ConcurrentVersionHistory<ITextDiff>(
                _authorProvider.ProvideAuthor(),
                name,
                new CommitMerger(_synchronizerProvider.ProvideFileSynchronizer(name, null)),
                historyId,
                existingCommits);
        }
    }
}