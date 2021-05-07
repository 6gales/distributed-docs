using DistributedDocs.VersionHistory;
using System;

namespace DistributedDocs.FileSystem
{
    public interface IConcurrentFileSynchronizer : IDisposable
    {
        void AddCommit(ITextDiff textDiff);
    }
}
