using DistributedDocs.DocumentChanges;

namespace DistributedDocs.FileSystem
{
    public sealed class ConcurrentFileSynchronizer : IConcurrentFileSynchronizer<ITextDiff>
    {
        public void AddChange(ITextDiff change)
        {
            throw new System.NotImplementedException();
        }
    }
}