using System.Collections.Generic;

namespace DistributedDocs.VersionHistory
{
    public interface ICommitMerger<T> where T : notnull
    {
        public void Merge(IReadOnlyCollection<ICommit<T>> history, ICommit<T> newCommit);
    }
}