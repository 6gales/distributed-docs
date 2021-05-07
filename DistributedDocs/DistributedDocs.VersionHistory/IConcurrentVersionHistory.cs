using System;

namespace DistributedDocs.VersionHistory
{
    public interface IConcurrentVersionHistory<T> where T : notnull
    {
        Guid Guid { get; }

        ICommit<T> CommitChange(T change);

        ICommit<T>? AddCommit(ICommit<T> commit);
    }
}
