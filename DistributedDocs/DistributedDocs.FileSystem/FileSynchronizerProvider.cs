using DistributedDocs.DocumentChanges;

namespace DistributedDocs.FileSystem
{
    public sealed class FileSynchronizerProvider : IFileSynchronizerProvider<ITextDiff>
    {
        public IConcurrentFileSynchronizer<ITextDiff> ProvideFileSynchronizer(string name, string? path)
        {
            throw new System.NotImplementedException();
        }
    }
}