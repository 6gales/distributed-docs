using System;
using System.Collections.Generic;

namespace DistributedDocs.VersionHistory
{
    public interface IConcurrentVersionHistory<T> where T : notnull
    {
        Guid Guid { get; }

        string Name { get; }

        IReadOnlyCollection<ICommit<T>> History { get; }

        ICommit<T> CommitChange(T change);

        ICommit<T>? AddCommit(ICommit<T> commit);
    }
}
