namespace DistributedDocs.VersionHistory
{
    public interface IConcurrentVersionHistory
    {
        void AddCommit(ITextDiff textDiff);
    }
}
