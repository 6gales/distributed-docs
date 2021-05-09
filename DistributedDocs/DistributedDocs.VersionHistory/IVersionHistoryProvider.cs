using System;
using System.Collections.Generic;

namespace DistributedDocs.VersionHistory
{
    public interface IVersionHistoryProvider<T> where T : notnull
    {
        IConcurrentVersionHistory<T> ProvideHistory(string name, string? path = null);

        IConcurrentVersionHistory<T> FromExisting(Guid historyId, string name, IEnumerable<ICommit<T>> existingCommits);
    }
}
