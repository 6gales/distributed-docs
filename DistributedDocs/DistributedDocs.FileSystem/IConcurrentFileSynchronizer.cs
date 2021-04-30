using DistributedDocs.VersionHistory;

namespace DistributedDocs.FileSystem
{
    public interface IConcurrentFileSynchronizer
    {
        void AddCommit(ITextDiff textDiff);
    }
}
