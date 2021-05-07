namespace DistributedDocs.FileSystem
{
    interface IFileSynchronizerProvider
    {
        IConcurrentFileSynchronizer Provide(string name, string? path = null);
    }
}
