namespace DistributedDocs.FileSystem
{
    public interface IConcurrentFileSynchronizer<in T> where T : notnull
    {
        void AddChange(T change);
    }
}
