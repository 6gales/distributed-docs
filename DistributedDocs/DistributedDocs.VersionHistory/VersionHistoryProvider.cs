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
                _synchronizerProvider.ProvideFileSynchronizer(name, path));
        }
    }
}