using System;

namespace DistributedDocs.FileSystem
{
    public interface IConcurrentFileSynchronizer<in T> : IDisposable where T : notnull
    {
        void AddChange(T change);
    }
}
