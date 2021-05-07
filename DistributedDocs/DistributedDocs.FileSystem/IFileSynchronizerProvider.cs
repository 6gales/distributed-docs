namespace DistributedDocs.FileSystem
{
    public interface IFileSynchronizerProvider<in T> where T : notnull
    {
        IConcurrentFileSynchronizer<T> ProvideFileSynchronizer(string name, string? path = null);
    }
}
