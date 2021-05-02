namespace DistributedDocs.VersionHistory
{
    public interface IConcurrentVersionHistory<T> where T : notnull
    {
        ICommit<T> CommitChange(T change);

        ICommit<T>? AddCommit(ICommit<T> commit);
    }
}
